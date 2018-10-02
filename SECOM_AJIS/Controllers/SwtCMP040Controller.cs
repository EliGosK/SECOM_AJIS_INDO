using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Controllers
{
    public class SwtCMP040Controller : SwtCommonController
    {
        //
        // GET: /SwtCMP040/

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
        ///     doMailProcess:
        ///         MailTo: NULL
        ///         Subject: ""
        ///         Message: ""
        ///         
        ///Expected:
        ///     MSG0007: "This field was required: MailTo."
        ///</summary>
        public string Case1() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            doEmailProcess param = new doEmailProcess();
            param.MailTo = null;
            param.Subject = "";
            param.Message = "";
            string expected = "MSG0007";
            string actual;

            try {
                actual = target.SendMail(param).ToString();
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check
        ///     
        ///Parameters:
        ///     doMailProcess:
        ///         MailTo: ""
        ///         Subject: null
        ///         Message: ""
        ///         
        ///Expected:
        ///     MSG0007: "This field was required: Subject."
        ///</summary>
        public string Case2() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            doEmailProcess param = new doEmailProcess();
            param.MailTo = "";
            param.Subject = null;
            param.Message = "";
            string expected = "MSG0007";
            string actual;

            try {
                actual = target.SendMail(param).ToString();
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check
        ///     
        ///Parameters:
        ///     doMailProcess:
        ///         MailTo: ""
        ///         Subject: ""
        ///         Message: null
        ///         
        ///Expected:
        ///     MSG0007: "This field was required: Message."
        ///</summary>
        public string Case3() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            doEmailProcess param = new doEmailProcess();
            param.MailTo = "";
            param.Subject = "";
            param.Message = null;
            string expected = "MSG0007";
            string actual;

            try {
                actual = target.SendMail(param).ToString();
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check
        ///     
        ///Parameters:
        ///     doMailProcess:
        ///         MailTo: akat@csithai.com
        ///         MailFrom: secomajis@gmail.com
        ///         MailFromAlias: SECOM-AJIS
        ///         Subject: test
        ///         Message: last case
        ///         
        ///Expected:
        ///     From: akat@csithai.com
        ///     From (Display name): SECOM-AJIS
        ///     Header (Subject): test
        ///     Body (Content): last case
        ///     
        ///</summary>
        public string Case4() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            doEmailProcess param = new doEmailProcess();
            param.MailTo = "akat@csithai.com";
            param.MailFrom = "secomajis@gmail.com";
            param.MailFromAlias = "SECOM-AJIS";
            param.Subject = "test";
            param.Message = "last case";
            string expected = null;
            string actual = null;

            try {
                target.SendMail(param).ToString();
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }
    }
}