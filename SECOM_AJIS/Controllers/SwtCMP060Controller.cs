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
    public class SwtCMP060Controller : SwtCommonController
    {
        //
        // GET: /SwtCMP060/

        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Check input manual flag
        ///     
        ///Parameters:
        ///     bManualFlag: 0
        ///     bSuspendFlag: 0
        ///     Current date: 8-Aug-2011
        ///         
        ///Expected:
        ///     Do nothing
        ///</summary>
        public string Case1() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = false;
            bool bSuspendFlag = false;
            //Current date = 8-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Check input manual flag
        ///     
        ///Parameters:
        ///     bManualFlag: 0
        ///     bSuspendFlag: 0
        ///     Current date: 8-Aug-2011
        ///         
        ///Expected:
        ///     Do nothing
        ///</summary>
        public string Case2() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = false;
            bool bSuspendFlag = false;
            //Current date = 8-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
                
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Resume system - There is batch running
        ///     
        ///Parameters:
        ///     bManualFlag: 0
        ///     bSuspendFlag: 0
        ///     Current date: 5-Aug-2011
        ///         
        ///Expected:
        ///     Write error log:
        ///         Desc : Cannot resume system because batch process is running
        ///         Create by : SYSTEM
        ///         
        ///     Write window log:
        ///         EventType : Warning
        ///         Message : Cannot Suspend/Resume process
        ///         
        ///</summary>
        public string Case3() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = false;
            bool bSuspendFlag = false;
            //Current date = 5-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Resume system - There is no batch running
        ///     
        ///Parameters:
        ///     bManualFlag: 0
        ///     bSuspendFlag: 0
        ///     Current date: 5-Aug-2011
        ///
        ///Procedure:
        ///     Update tbm_BatchProcess
        ///     SET BatchStatus = 1
        ///     Where BatchCode in ('01','02','03')
        ///     
        ///Expected:
        ///     Update configuration value of suspend flag
        ///         tbs_Configuration.ConfigValue : 0
        ///         Expectation: Test cast 4
        ///     
        ///     Write window log:
        ///         EventType : Information
        ///         Message : The system resume
        ///         
        ///</summary>
        public string Case4() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = false;
            bool bSuspendFlag = false;
            //Current date = 5-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Suspend system
        ///     
        ///Parameters:
        ///     bManualFlag: 0
        ///     bSuspendFlag: 1
        ///     Current date: 6-Aug-2011
        ///
        ///Expected:
        ///     Update configuration value of suspend flag
        ///         tbs_Configuration.ConfigValue : 1
        ///         Expectation: Test cast 5
        ///     
        ///     Write window log
        ///         EventType : Information
        ///         Message : The system suspended
        ///         
        ///</summary>
        public string Case5() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = false;
            bool bSuspendFlag = true;
            //Current date = 6-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Resume system - There is batch running
        ///     
        ///Parameters:
        ///     bManualFlag: 1
        ///     bSuspendFlag: 0
        ///     Current date: 8-Aug-2011
        ///
        ///Procedure:
        ///     Update tbm_BatchProcess
        ///     Set BatchStatus = 2
        ///     Where BatchCode in ('01','02','03')
        /// 
        ///Expected:
        ///     Write error log:
        ///         Desc : Cannot resume system because batch process is running
        ///         Create by : SYSTEM
        ///        
        ///     Write window log:
        ///         EventType : Warning
        ///         Message : Cannot Suspend/Resume process
        ///         
        ///</summary>
        public string Case6() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = true;
            bool bSuspendFlag = false;
            //Current date = 8-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Resume system - There is no batch running
        ///     
        ///Parameters:
        ///     bManualFlag: 1
        ///     bSuspendFlag: 0
        ///     Current date: 8-Aug-2011
        ///
        ///Procedure:
        ///     Update tbm_BatchProcess
        ///     Set BatchStatus = 1
        ///     Where BatchCode in ('01','02','03')
        /// 
        ///Expected:
        ///     Update configuration value of suspend flag
        ///         tbs_Configuration.ConfigValue : 0
        ///         Expectation: Test cast 7
        ///         
        ///     Write window log
        ///         EventType : Information
        ///         Message : The system resume
        ///         
        ///</summary>
        public string Case7() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = true;
            bool bSuspendFlag = false;
            //Current date = 8-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Suspend system
        ///     
        ///Parameters:
        ///     bManualFlag: 1
        ///     bSuspendFlag: 1
        ///     Current date: 8-Aug-2011
        /// 
        ///Expected:
        ///     Update configuration value of suspend flag
        ///         tbs_Configuration.ConfigValue : 1
        ///         Expectation: Test cast 8
        ///         
        ///     Write window log:
        ///         EventType : Information
        ///         Message : The system suspended
        ///         
        ///</summary>
        public string Case8() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bManualFlag = true;
            bool bSuspendFlag = true;
            //Current date = 8-Aug-2011

            string expected = null;
            string actual = null;

            try {
                target.UpdateSystemStatus(bSuspendFlag, bManualFlag, "");
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_String(expected, actual));
        }

    }
}