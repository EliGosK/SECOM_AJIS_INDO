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
    public class SwtCMP061Controller : SwtCommonController
    {
        //
        // GET: /SwtCMP020/

        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Resume system
        ///     
        ///Pre-Condition
        ///     Current date: 04/08/2011
        ///     Current time: 07:00
        /// 
        ///Expected:
        ///     SuspendFlag : 0
        ///     ResumeServiceDateTime: 04/08/2011  18:00:00
        ///     SuspendServiceDateTime: 04/08/2011  9:00:00
        ///</summary>
        public string Case1() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = false;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 4, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 4, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status  - Resume system
        ///     
        ///Pre-Condition
        ///     Current date: 04/08/2011
        ///     Current time: 15:00
        /// 
        ///Expected:
        ///     SuspendFlag : 0
        ///     ResumeServiceDateTime: 04/08/2011  18:00:00
        ///     SuspendServiceDateTime: 05/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case2() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = false;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 4, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 5, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status  - Resume system
        ///     
        ///Pre-Condition
        ///     Current date: 04/08/2011
        ///     Current time: 20:00
        /// 
        ///Expected:
        ///     SuspendFlag : 0
        ///     ResumeServiceDateTime: 05/08/2011  18:00:00
        ///     SuspendServiceDateTime: 05/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case3() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = false;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 5, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 5, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status  - Resume system
        ///     
        ///Pre-Condition
        ///     Current date: 05/08/2011
        ///     Current time: 7:00
        /// 
        ///Expected:
        ///     SuspendFlag : 0
        ///     ResumeServiceDateTime: 05/08/2011  18:00:00
        ///     SuspendServiceDateTime: 05/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case4() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = false;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 5, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 5, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status  - Resume system
        ///     
        ///Pre-Condition
        ///     Current date: 05/08/2011
        ///     Current time: 15:00
        /// 
        ///Expected:
        ///     SuspendFlag : 0
        ///     ResumeServiceDateTime: 05/08/2011  18:00:00
        ///     SuspendServiceDateTime: 06/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case5() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = false;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 5, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 6, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status  - Resume system
        ///     
        ///Pre-Condition
        ///     Current date: 05/08/2011
        ///     Current time: 20:00
        /// 
        ///Expected:
        ///     SuspendFlag : 0
        ///     ResumeServiceDateTime: 06/08/2011  18:00:00
        ///     SuspendServiceDateTime: 06/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case6() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = false;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 6, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 6, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status  - Resume system
        ///     
        ///Pre-Condition:
        ///     Current date: 06/08/2011
        ///     Current time: 7:00
        ///     
        ///Expected:
        ///     SuspendFlag : 0
        ///     ResumeServiceDateTime: 10/08/2011  18:00:00
        ///     SuspendServiceDateTime: 10/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case7() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = false;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 10, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 10, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Suspend system
        ///     
        ///Pre-Condition
        ///     Current date: 04/08/2011
        ///     Current time: 7:00
        /// 
        ///Procedure:
        ///     Update tbs_Configuration
        ///     Set ConfigValue = 1
        ///     Where ConfigName = 'SuspendFlag'
        /// 
        ///Expected:
        ///     SuspendFlag : 1
        ///     ResumeServiceDateTime: 04/08/2011  18:00:00
        ///     SuspendServiceDateTime: 04/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case8() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = true;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 4, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 4, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Suspend system
        ///     
        ///Pre-Condition
        ///     Current date: 04/08/2011
        ///     Current time: 15:00
        /// 
        ///Expected:
        ///     SuspendFlag : 1
        ///     ResumeServiceDateTime: 04/08/2011  18:00:00
        ///     SuspendServiceDateTime: 05/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case9() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = true;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 4, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 5, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Suspend system
        ///     
        ///Pre-Condition
        ///     Current date: 04/08/2011
        ///     Current time: 20:00
        /// 
        ///Expected:
        ///     SuspendFlag : 1
        ///     ResumeServiceDateTime: 05/08/2011  18:00:00
        ///     SuspendServiceDateTime: 05/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case10() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = true;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 5, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 5, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 10, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Suspend system
        ///     
        ///Pre-Condition
        ///     Current date: 05/08/2011
        ///     Current time: 7:00
        /// 
        ///Expected:
        ///     SuspendFlag : 1
        ///     ResumeServiceDateTime: 05/08/2011  18:00:00
        ///     SuspendServiceDateTime: 05/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case11() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = true;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 5, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 5, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 11, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Suspend system
        ///     
        ///Pre-Condition
        ///     Current date: 05/08/2011
        ///     Current time: 15:00
        /// 
        ///Expected:
        ///     SuspendFlag : 1
        ///     ResumeServiceDateTime: 05/08/2011  18:00:00
        ///     SuspendServiceDateTime: 06/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case12() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = true;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 5, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 6, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 12, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Suspend system
        ///     
        ///Pre-Condition
        ///     Current date: 05/08/2011
        ///     Current time: 20:00
        /// 
        ///Expected:
        ///     SuspendFlag : 1
        ///     ResumeServiceDateTime: 06/08/2011  18:00:00
        ///     SuspendServiceDateTime: 06/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case13() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = true;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 6, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 6, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 13, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get system status - Suspend system
        ///     
        ///Pre-Condition
        ///     Current date: 06/08/2011
        ///     Current time: 7:00
        /// 
        ///Expected:
        ///     SuspendFlag : 1
        ///     ResumeServiceDateTime: 10/08/2011  18:00:00
        ///     SuspendServiceDateTime: 10/08/2011  9:00:00
        ///     
        ///</summary>
        public string Case14() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemStatus> expected = new List<doSystemStatus>();
            doSystemStatus status = new doSystemStatus();
            status.SuspendFlag = true;
            status.ResumeServiceDateTime = new DateTime(2011, 8, 10, 18, 0, 0);
            status.SuspendServiceDateTime = new DateTime(2011, 8, 10, 9, 0, 0);

            List<doSystemStatus> actual = null;

            try {
                actual = target.GetSystemStatus();
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 14, expected, actual, CompareResult_Object(expected, actual));
        }
    }
}