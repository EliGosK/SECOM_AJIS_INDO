using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Controllers
{
    //
    // GET: /SwtCMP070/

    public class SwtMAP010Controller : SwtCommonController {
        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check
        ///     
        ///Parameters:
        ///     custCode: NULL
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: RunningNo."
        ///</summary>
        public string Case1() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string custCode = null;
            string expected = "MSG0007";
            string actual = null;

            try {
                target.ManageCustomerInformation(custCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get customer data (How does the system perform if it cannot get customer data)
        ///     
        ///Parameters:
        ///     custCode: "C0000000000"
        ///         
        ///Expected:
        ///     MSG2006: "Customer code not found, C0000000000."
        ///</summary>
        public string Case2() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string custCode = "C0000000000";
            string expected = "MSG2006";
            string actual = null;

            try {
                target.ManageCustomerInformation(custCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Update customer data when customer status = 0 (New customer)
        ///     
        ///Parameters:
        ///     custCode: "C0000000027"
        ///
        ///Precondition
        ///     Login user: 500576
        ///     Process datetime: 2011-11-14 09:20:00.000
        ///         
        ///Expected:
        ///     tbm_Customer:
        ///         custStatus: 1
        ///         UpdateDate: 2011-11-14 09:20:00.000
        ///         UpdateBy: 500576
        ///</summary>
        public string Case3() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string custCode = "C0000000027";
            string expected = null;
            string actual = null;

            CommonUtil.dsTransData.dtUserData.EmpNo = "500576";
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011,11,14,9,20,0);

            try {
                target.ManageCustomerInformation(custCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Update customer data when customer status = 1 (Existing customer)
        ///     
        ///Parameters:
        ///     custCode: "C0000000014"
        ///
        ///Precondition
        ///     Login user: 500576
        ///     Process datetime: 2011-11-19  10:30:00.000
        ///         
        ///Expected:
        ///     record of custCode = C0000000014 tbm_Customer does not change
        ///</summary>
        public string Case4() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string custCode = "C0000000014";
            string expected = null;
            string actual = null;

            CommonUtil.dsTransData.dtUserData.EmpNo = "500576";
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 11, 19, 10, 30, 0);

            try {
                target.ManageCustomerInformation(custCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

    }
}