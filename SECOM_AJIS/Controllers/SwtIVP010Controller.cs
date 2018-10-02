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
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Controllers
{
    public class SwtIVP010Controller : SwtCommonController
    {
        //
        // GET: /SwtIVP010/

        public string index()
        {
            string result = "";
            List<string> resultLst = new List<string>();

            resultLst.Add(Case1_1());
            resultLst.Add(Case1_2());
            resultLst.Add(Case2());

            resultLst.Add(Case3());
            resultLst.Add(Case4());

            resultLst.Add(Case5());
            resultLst.Add(Case6());

            resultLst.Add(Case7());
            Case1_2(); // Pre Condition
            resultLst.Add(Case8());

            resultLst.Add(Case9());
            resultLst.Add(Case10());
            resultLst.Add(Case11());
            resultLst.Add(Case12());

            result = CommonUtil.TextList(resultLst.ToArray(), "<br />");

            return result;
        }


        ///<summary>
        ///Purpose   : not found contract code
        ///Parameters: Input01
        ///Expected  : - blnExistContractCode = 1 (true)
        ///</summary>
        ///
        public string Case1_1()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = true;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.NewBooking(Input01());
                expected = expectedObj.ToString().ToUpper();
                actual = actualObj.ToString().ToUpper();

            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, "1-1", expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : not found contract code
        ///Parameters: Input02
        ///Expected  : - insert data in dobooking to Result1
        ///            - blnExistContractCode = 0 (false)
        ///</summary>
        ///
        public string Case1_2()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = false;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                CommonUtil.dsTransData.dtTransHeader.ScreenID = ScreenID.C_SCREEN_ID_FN99;
                actualObj = invenhandler.NewBooking(Input02());

                expected = expectedObj.ToString().ToUpper();
                actual = actualObj.ToString().ToUpper();
                result = String.Format(RESULT_FORMAT, "1-2", expected, actual, CompareResult_bool(expectedObj, actualObj));
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            //result = String.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_bool(expected, actual));

            return result;
        }

        ///<summary>
        ///Purpose   : result after booking from Screen 
        ///Parameters: Input02
        ///Expected  : - insert data in dobooking to Result2
        ///            - blnExistContractCode = 0 (false)
        ///</summary>
        ///
        public string Case2()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = false;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                CommonUtil.dsTransData.dtTransHeader.ScreenID = ScreenID.C_SCREEN_ID_FQ99;
                actualObj = invenhandler.NewBooking(Input02());

                actual = actualObj.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : not found contract code
        ///Parameters: Input03
        ///Expected  : blnExistContractCode = 0 (false)
        ///</summary>
        ///
        public string Case3()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = false;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.ChangeExpectedStartServiceDate(Input03());

                actual = actualObj.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : change start service date
        ///Parameters: Input04
        ///Expected  : - update  tbt_InventoryBooking.ExpectedstartServiceDate from input3
        ///            - blnExistContractCode = 1 (true)
        ///</summary>
        ///
        public string Case4()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = true;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.ChangeExpectedStartServiceDate(Input04());

                actual = actualObj.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : not found contract code
        ///Parameters: Input03
        ///Expected  : blnExistContractCode = 0 (false)
        ///</summary>
        ///
        public string Case5()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = false;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.CancelBooking(Input03());

                actual = actualObj.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : cancel booking
        ///Parameters: Input04
        ///Expected  : - delete data from tbt_inventoryBookingDeatail and tbt_inventoryBooking where ContractCode = doBooking.ContractCode
        ///            - Write transaction log
        ///            - blnExistContractCode = 1 (true)
        ///</summary>
        ///
        public string Case6()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = true;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.CancelBooking(Input04());

                actual = actualObj.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : not found contract code
        ///Parameters: Input03
        ///Expected  : - blnExistContractCode = 0 (false)
        ///</summary>
        ///
        public string Case7()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = false;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.Rebooking(Input03());

                actual = actualObj.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : rebooking effect
        ///Parameters: Input02
        ///Expected  : - update  StockOutQty = 0 in tbt_inventoryBookingDetail where ContractCode = input2.ContractCode
        ///            - Write transaction log
        ///            - blnExistContractCode = 1 (true)
        ///</summary>
        ///
        public string Case8()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool? expectedObj = true;
            bool? actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.Rebooking(Input02());

                actual = actualObj.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_bool(expectedObj, actualObj));

            return result;
        }

        ///<summary>
        ///Purpose   : not found contract code
        ///Parameters: Input03
        ///Expected  : blnExistContractCode = 0 (false)
        ///</summary>
        ///
        public string Case9()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool expectedObj = false;
            doBooking actualObj = null;

            string expected = expectedObj.ToString().ToUpper();
            string actual = "";

            try
            {
                actualObj = invenhandler.UpdateStockOutInstrument(Input03());

                actual = actualObj.blnExistContractCode.ToString().ToUpper();
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_bool(expectedObj, actualObj.blnExistContractCode));

            return result;
        }

        ///<summary>
        ///Purpose   : FirstInstallationCompleteFlag is zero
        ///Parameters: Input04
        ///Expected  : - blnExistContractCode = 1
        ///            - blnFirstInstallationCompleteFlag =0
        ///</summary>
        ///
        public string Case10()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool expectedObj_01 = true, expectedObj_02 = false;
            doBooking actualObj = null;

            string expected01 = expectedObj_01.ToString().ToUpper();
            string expected02 = expectedObj_02.ToString().ToUpper();
            string actual_01 = "", actual_02 = "", actual = "", expected = "";

            string resFormat = "blnExistContractCode = {0}, blnFirstInstallationCompleteFlag = {1}";

            try
            {
                actualObj = invenhandler.UpdateStockOutInstrument(Input04());

                actual_01 = actualObj.blnExistContractCode.ToString().ToUpper();
                actual_02 = actualObj.blnFirstInstallCompleteFlag.ToString().ToUpper();

                expected = String.Format(resFormat, expectedObj_01, expectedObj_02);
                actual = String.Format(resFormat, actual_01, actual_02);
            }
            catch (ApplicationErrorException ex)
            {
                actual_01 = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual_01 = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 10, expected, actual
                , String.Format(resFormat
                    , CompareResult_bool(expectedObj_01, actualObj.blnExistContractCode)
                    , CompareResult_bool(expectedObj_02, actualObj.blnFirstInstallCompleteFlag)));

            return result;
        }

        ///<summary>
        ///Purpose   : Stock Out < Booking
        ///Parameters: Input05
        ///Expected  : - blnExistContractCode = 1
        ///            - blnFirstInstallationCompleteFlag = 1
        ///            - Result 3
        ///</summary>
        ///
        public string Case11()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool expectedObj_01 = true, expectedObj_02 = true;
            doBooking actualObj = null;

            string expected01 = expectedObj_01.ToString().ToUpper();
            string expected02 = expectedObj_02.ToString().ToUpper();
            string actual_01 = "", actual_02 = "", actual = "", expected = "";

            string resFormat = "blnExistContractCode = {0}, blnFirstInstallationCompleteFlag = {1}";

            try
            {
                actualObj = invenhandler.UpdateStockOutInstrument(Input05());

                actual_01 = actualObj.blnExistContractCode.ToString().ToUpper();
                actual_02 = actualObj.blnFirstInstallCompleteFlag.ToString().ToUpper();

                expected = String.Format(resFormat, expectedObj_01, expectedObj_02);
                actual = String.Format(resFormat, actual_01, actual_02);
            }
            catch (ApplicationErrorException ex)
            {
                actual_01 = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual_01 = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 11, expected, actual
                , String.Format(resFormat
                    , CompareResult_bool(expectedObj_01, actualObj.blnExistContractCode)
                    , CompareResult_bool(expectedObj_02, actualObj.blnFirstInstallCompleteFlag)));

            return result;
        }

        ///<summary>
        ///Purpose   : Stock Out < Booking
        ///Parameters: Input04
        ///Expected  : - blnExistContractCode = 1
        ///            - blnFirstInstallationCompleteFlag = 1
        ///            - Result 3
        ///</summary>
        ///
        public string Case12()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string result = "";

            bool expectedObj_01 = true, expectedObj_02 = true;
            doBooking actualObj = null;

            string expected01 = expectedObj_01.ToString().ToUpper();
            string expected02 = expectedObj_02.ToString().ToUpper();
            string actual_01 = "", actual_02 = "", actual = "", expected = "";

            string resFormat = "blnExistContractCode = {0}, blnFirstInstallationCompleteFlag = {1}";

            try
            {
                actualObj = invenhandler.UpdateStockOutInstrument(Input04());

                actual_01 = actualObj.blnExistContractCode.ToString().ToUpper();
                actual_02 = actualObj.blnFirstInstallCompleteFlag.ToString().ToUpper();

                expected = String.Format(resFormat, expectedObj_01, expectedObj_02);
                actual = String.Format(resFormat, actual_01, actual_02);
            }
            catch (ApplicationErrorException ex)
            {
                actual_01 = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual_01 = ex.StackTrace;
            }

            result = String.Format(RESULT_FORMAT, 12, expected, actual
                , String.Format(resFormat
                    , CompareResult_bool(expectedObj_01, actualObj.blnExistContractCode)
                    , CompareResult_bool(expectedObj_02, actualObj.blnFirstInstallCompleteFlag)));

            return result;
        }

        #region Input Data
        public doBooking Input01()
        {
            return new doBooking()
            {
                ContractCode = "CT0000000001",
                ExpectedStartServiceDate = new DateTime(2012, 05, 24, 17, 04, 00),
                InstrumentCode = new List<string>(),
                InstrumentQty = new List<int>(),
                blnExistContractCode = false,
                blnFirstInstallCompleteFlag = false
            };
        }

        public doBooking Input02()
        {
            var obj = new doBooking()
            {
                ContractCode = "CT0000000002",
                ExpectedStartServiceDate = new DateTime(2012, 05, 24, 17, 04, 00),
                InstrumentCode = new List<string>(),
                InstrumentQty = new List<int>(),
                blnExistContractCode = false,
                blnFirstInstallCompleteFlag = false
            };

            obj.InstrumentCode.Add("1001");
            obj.InstrumentCode.Add("1002");
            obj.InstrumentCode.Add("1003");

            obj.InstrumentQty.Add(20);
            obj.InstrumentQty.Add(30);
            obj.InstrumentQty.Add(40);

            return obj;
        }

        public doBooking Input03()
        {
            doBooking res = new doBooking()
            {
                ContractCode = "CT0000000003"
                , ExpectedStartServiceDate = new DateTime(2012, 05, 25, 09, 49, 00)
                , InstrumentCode = new List<string>()
                , InstrumentQty = new List<int>()
                , blnExistContractCode = false
                , blnFirstInstallCompleteFlag = false
            };

            return res;
        }

        public doBooking Input04()
        {
            var obj = new doBooking()
            {
                ContractCode = "CT0000000002",
                ExpectedStartServiceDate = new DateTime(2012, 03, 29, 00, 00, 00),
                InstrumentCode = new List<string>(),
                InstrumentQty = new List<int>(),
                blnExistContractCode = false,
                blnFirstInstallCompleteFlag = false
            };

            obj.InstrumentCode.Add("1001");
            obj.InstrumentCode.Add("1002");
            obj.InstrumentCode.Add("1003");

            obj.InstrumentQty.Add(20);
            obj.InstrumentQty.Add(30);
            obj.InstrumentQty.Add(40);

            return obj;
        }

        public doBooking Input05()
        {
            var obj = new doBooking()
            {
                ContractCode = "CT0000000002",
                ExpectedStartServiceDate = new DateTime(2012, 03, 29, 00, 00, 00),
                InstrumentCode = new List<string>(),
                InstrumentQty = new List<int>(),
                blnExistContractCode = false,
                blnFirstInstallCompleteFlag = false
            };

            obj.InstrumentCode.Add("1001");
            obj.InstrumentCode.Add("1002");
            obj.InstrumentCode.Add("1003");

            obj.InstrumentQty.Add(20);
            obj.InstrumentQty.Add(20);
            obj.InstrumentQty.Add(40);

            return obj;
        }

        #endregion
    }
}