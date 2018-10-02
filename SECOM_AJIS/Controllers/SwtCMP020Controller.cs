using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Controllers {
    public class SwtCMP020Controller : SwtCommonController {
        //
        // GET: /SwtCMP020/

        public string index()
        {
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
        ///     Check mandatory filelds.
        ///     
        ///Parameters:
        ///     doTransactionLog:
        ///         TransactionType: NULL
        ///         TableName: ""
        ///         TableData: ""
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: TransactionType."
        ///</summary>
        public string Case1() {
            ILogHandler target = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            doTransactionLog param = new doTransactionLog();
            //param.TransactionType = "";
            param.TableData = "";
            param.TableName = "";
            param.TransactionType = null;
            string expected = "MSG0007";
            string actual = null;

            try {
                target.WriteTransactionLog(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Check mandatory filelds.
        ///     
        ///Parameters:
        ///     doTransactionLog:
        ///         TransactionType: doTransactionLog.eTransactionType.Insert
        ///         TableName: NULL
        ///         TableData: ""
        ///         
        ///Expected:
        ///     MSG0007: "This field was required: TableName."
        ///</summary>
        public string Case2() {
            ILogHandler target = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            doTransactionLog param = new doTransactionLog();
            param.TransactionType = doTransactionLog.eTransactionType.Insert;
            param.TableName = null;
            param.TableData = "";

            string expected = "MSG0007";
            string actual = null;

            try {
                target.WriteTransactionLog(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Check mandatory filelds.
        ///     
        ///Parameters:
        ///     doTransactionLog:
        ///         TransactionType: doTransactionLog.eTransactionType.Insert
        ///         TableName: ""
        ///         TableData: NULL
        ///         
        ///Expected:
        ///     MSG0007: "This field was required: TableData."
        ///</summary>
        public string Case3()
        {
            ILogHandler target = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            doTransactionLog param = new doTransactionLog();
            param.TransactionType = doTransactionLog.eTransactionType.Insert;
            param.TableName = "";
            param.TableData = null;

            string expected = "MSG0007";
            string actual = null;

            try {
                target.WriteTransactionLog(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }


        ///<summary>
        ///Purpose:
        ///     Check mandatory filelds.
        ///     
        ///Parameters:
        ///     doTransactionLog:
        ///         refer to "SECOM-AJIS-STC.CMP020-Process of creating log" tab Test Data
        ///         
        ///Expected:
        ///     refer to "SECOM-AJIS-STC.CMP020-Process of creating log" tab Test Data
        ///</summary>
        public string Case4() {
            ILogHandler target = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            doTransactionLog param = new doTransactionLog();
            string expected = null;
            string actual = null;

            try
            {
                // 1.
                param.TransactionType = doTransactionLog.eTransactionType.Insert;
                param.TableName = "Table1";
                param.TableData = "Datatable object";
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES010";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011,1,1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000001";
                target.WriteTransactionLog(param);

                // 2.
                param.TransactionType = doTransactionLog.eTransactionType.Update;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES011";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 2, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000002";
                target.WriteTransactionLog(param);

                // 3.
                param.TransactionType = doTransactionLog.eTransactionType.Delete;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES012";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 3, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000003";
                target.WriteTransactionLog(param);

                // 4.
                param.TransactionType = doTransactionLog.eTransactionType.Insert;
                param.TableName = "Table2";
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES013";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 4, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000004";
                target.WriteTransactionLog(param);

                // 5.
                param.TransactionType = doTransactionLog.eTransactionType.Update;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES014";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 5, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000005";
                target.WriteTransactionLog(param);

                // 6.
                param.TransactionType = doTransactionLog.eTransactionType.Delete;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES015";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 6, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000006";
                target.WriteTransactionLog(param);

                // 7.
                param.TransactionType = doTransactionLog.eTransactionType.Insert;
                param.TableName = "Table3";
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES016";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 7, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000007";
                target.WriteTransactionLog(param);

                // 8.
                param.TransactionType = doTransactionLog.eTransactionType.Update;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES017";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 1, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000008";
                target.WriteTransactionLog(param);

                // 9.
                param.TransactionType = doTransactionLog.eTransactionType.Delete;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES018";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 2, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000009";
                target.WriteTransactionLog(param);

                // 10.
                param.TransactionType = doTransactionLog.eTransactionType.Insert;
                param.TableName = "Table4";
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES019";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 3, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000010";
                target.WriteTransactionLog(param);

                // 11.
                param.TransactionType = doTransactionLog.eTransactionType.Update;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES020";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 4, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000011";
                target.WriteTransactionLog(param);

                // 12.
                param.TransactionType = doTransactionLog.eTransactionType.Delete;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES021";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 5, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000012";
                target.WriteTransactionLog(param);

                // 13.
                param.TransactionType = doTransactionLog.eTransactionType.Insert;
                param.TableName = "Table5";
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES022";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 6, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000013";
                target.WriteTransactionLog(param);

                // 14.
                param.TransactionType = doTransactionLog.eTransactionType.Update;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES023";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 7, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000014";
                target.WriteTransactionLog(param);

                // 15.
                param.TransactionType = doTransactionLog.eTransactionType.Delete;
                CommonUtil.dsTransData.dtTransHeader.ScreenID = "TES024";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 1, 1);
                CommonUtil.dsTransData.dtUserData.EmpNo = "000015";
                target.WriteTransactionLog(param);

            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }
    }
}