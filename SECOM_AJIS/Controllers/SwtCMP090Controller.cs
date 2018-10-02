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
    //
    // GET: /SwtCMP090/

    public class SwtCMP090Controller : SwtCommonController
    {
        public string index()
        {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Print ERROR log
        ///     
        ///Parameters:
        ///     strEventType: EventType.C_EVENT_TYPE_ERROR
        ///     strMessage: "Duplicate key error"
        ///         
        ///Expected:
        ///     Date: Current date.
        ///     Time: Current time.
        ///     Source: 'SECOM-AJIS web application'
        ///     Category: 'None'
        ///     Event: Error
        ///     User: 'None'
        ///     Computer: Name of computer
        ///     Description: Duplicate key error
        ///     Data: empty value
        ///</summary>
        public string Case1() {
            ILogHandler target = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            string strEventType = EventType.C_EVENT_TYPE_ERROR;
            string strMessage = "Duplicate key error";
            string expected = null;
            string actual = null;

            try {
                target.WriteWindowLog(strEventType, strMessage, EventID.C_EVENT_ID_INTERNAL_ERROR);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Print WARNING log
        ///Parameters:
        ///     strEventType: EventType.C_EVENT_TYPE_WARNING
        ///     strMessage: "There are some error at night batch"
        ///         
        ///Expected:
        ///     Date: Current date.
        ///     Time: Current time.
        ///     Source: 'SECOM-AJIS web application'
        ///     Category: 'None'
        ///     Event: Warning
        ///     User: 'None'
        ///     Computer: Name of computer
        ///     Description: There are some error at night batch
        ///     Data: empty value
        ///</summary>
        public string Case2() {
            ILogHandler target = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            string strEventType = EventType.C_EVENT_TYPE_WARNING;
            string strMessage = "There are some error at night batch";
            string expected = null;
            string actual = null;

            try {
                target.WriteWindowLog(strEventType, strMessage, EventID.C_EVENT_ID_BATCH_ERROR);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Print INFORMATION log
        ///Parameters:
        ///     strEventType: EventType.C_EVENT_TYPE_INFORMATION
        ///     strMessage: "Night batch is started"
        ///         
        ///Expected:
        ///     Date: Current date.
        ///     Time: Current time.
        ///     Source: 'SECOM-AJIS web application'
        ///     Category: 'None'
        ///     Event: Information
        ///     User: 'None'
        ///     Computer: Name of computer
        ///     Description: Night batch is started
        ///     Data: empty value
        ///</summary>
        public string Case3() {
            ILogHandler target = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            string strEventType = EventType.C_EVENT_TYPE_INFORMATION;
            string strMessage = "Night batch is started";
            string expected = null;
            string actual = null;

            try {
                target.WriteWindowLog(strEventType, strMessage, EventID.C_EVENT_ID_BATCH_START);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }
    }
}