
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Billing;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;


using System.Transactions;
using System.IO;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {

        //----------- Software test -----------//


        // [BLP010] Manage billing basic information
        public ActionResult CMS999_BLP010(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                // ManageBillingBasic

                List<doBillingTempBasic> input_list = new List<doBillingTempBasic>();

                switch (case_on)
                {
                    case "1":
                        input_list.Add(new doBillingTempBasic()
                        {
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 10000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0", // eq. tbt_BillingBasic.CalDailyFeeStatus
                            ContractTiming = 1,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            BillingClientCode = "0000000002",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "06",
                            BillingAmount = 1000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 1,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });


                        break;

                    case "2":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 15000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 1,
                            ProductTypeCode = "2",
                            CreditTerm = 1

                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingClientCode = "0000000002",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "06",
                            BillingAmount = 1000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 1,
                            ProductTypeCode = "2",
                            CreditTerm = 1


                        });



                        break;

                    case "3":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "2020",
                            PaymentMethod = "0",
                            ContractBillingType = "03",
                            BillingAmount = 10000,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 1,
                            ProductTypeCode = "6",
                            CreditTerm = 60

                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "2020",
                            PaymentMethod = "0",
                            ContractBillingType = "06",
                            BillingAmount = 9999.50M,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 1,
                            ProductTypeCode = "6",
                            CreditTerm = 60
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000002",
                            BillingOfficeCode = "2020",
                            PaymentMethod = "0",
                            ContractBillingType = "05",
                            BillingAmount = 55555.55M,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 1,
                            ProductTypeCode = "6",
                            CreditTerm = 60,

                        });


                        break;
                    case "4":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "01",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000001-001",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 15000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 2,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingClientCode = "0000000003",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 1000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 2,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        break;
                    case "5":
                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "01",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000001-001",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 10000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 3,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "02",
                            BillingClientCode = "0000000003",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000002-001",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 1000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 3,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingClientCode = "0000000004",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 5000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 3,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        break;
                    case "6":
                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "01",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000001-001",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 10000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 4,
                            ProductTypeCode = "2",
                            CreditTerm = 1

                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "03",
                            BillingClientCode = "0000000003",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000003-001",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 1000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 4,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "04",
                            BillingClientCode = "0000000004",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000004-001",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 5000,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 4,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });

                        break;
                    case "7":
                        //"Parameters: 
                        //- ContractCode = N0000000001
                        //- StartService = 2/05/2012
                        //- AdjustDate = 31/05/2012"

                        // ManageBillingBasicForStart
                        handlerBilling.ManageBillingBasicForStart("N0000000001", new DateTime(2012, 5, 2), new DateTime(2012, 5, 31));
                        break;
                    case "8":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "2020",
                            PaymentMethod = "0",
                            ContractBillingType = "03",
                            BillingAmount = 10000,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 4,
                            ProductTypeCode = "6",
                            CreditTerm = 60
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "2020",
                            PaymentMethod = "0",
                            ContractBillingType = "06",
                            BillingAmount = 9999.50M,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 4,
                            ProductTypeCode = "6",
                            CreditTerm = 60
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000002",
                            BillingOfficeCode = "2020",
                            PaymentMethod = "0",
                            ContractBillingType = "05",
                            BillingAmount = 55555.55M,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 4,
                            ProductTypeCode = "6",
                            CreditTerm = 60
                        });

                        break;

                    case "9":
                        //"Parameters: 
                        //- ContractCode = MA0000510312
                        //- StartService = 2/05/2012
                        //- AdjustDate = NULL"


                        handlerBilling.ManageBillingBasicForStart("MA0000510312", new DateTime(2012, 5, 2), null);


                        break;
                    case "10":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "2020",
                            BillingTargetCode = "0000000001-002",
                            ContractBillingType = "03",
                            BillingAmount = 5000,
                            ContractTiming = 5,
                            ProductTypeCode = "6",
                            ChangeFeeDate = new DateTime(2012, 6, 1)
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingClientCode = "0000000003",
                            BillingOfficeCode = "2020",
                            ContractBillingType = "03",
                            BillingAmount = 5000,
                            ContractTiming = 5,
                            ProductTypeCode = "6",
                            ChangeFeeDate = new DateTime(2012, 6, 1)
                        });



                        break;
                    case "11":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "01",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000001-001",
                            ContractBillingType = "01",
                            BillingAmount = 10000,
                            ContractTiming = 7,
                            ProductTypeCode = "2",
                            ChangeFeeDate = new DateTime(2012, 5, 16)

                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "03",
                            BillingClientCode = "0000000003",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000003-001",
                            ContractBillingType = "01",
                            BillingAmount = 5000,
                            ContractTiming = 7,
                            ProductTypeCode = "2",
                            ChangeFeeDate = new DateTime(2012, 5, 16)
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "04",
                            BillingClientCode = "0000000004",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000004-001",
                            ContractBillingType = "01",
                            BillingAmount = 5000,
                            ContractTiming = 7,
                            ProductTypeCode = "2",
                            ChangeFeeDate = new DateTime(2012, 5, 16)

                        });

                        break;
                    case "12":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "01",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000001-001",
                            ContractBillingType = "01",
                            BillingAmount = 5000,
                            ContractTiming = 7,
                            ProductTypeCode = "2",
                            ChangeFeeDate = new DateTime(2012, 6, 1)

                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "03",
                            BillingClientCode = "0000000003",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000003-001",
                            ContractBillingType = "01",
                            BillingAmount = 5000,
                            ContractTiming = 7,
                            ProductTypeCode = "2",
                            ChangeFeeDate = new DateTime(2012, 6, 1)
                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "N0000000001",
                            BillingOCC = "04",
                            BillingClientCode = "0000000004",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000004-001",
                            ContractBillingType = "01",
                            BillingAmount = 5000,
                            ContractTiming = 7,
                            ProductTypeCode = "2",
                            ChangeFeeDate = new DateTime(2012, 6, 1)
                        });

                        break;
                    case "13":

                        //Parameters: 
                        //- ContractCode = MA0000510312
                        //- StopDate = 01/07/2012
                        //- StopFee = 1000

                        // ManageBillingBasicForStop 

                        handlerBilling.ManageBillingBasicForStop("MA0000510312", new DateTime(2012, 7, 1), 1000);


                        break;
                    case "14":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingOCC = "01",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "2020",
                            BillingTargetCode = "0000000001-002",
                            PaymentMethod = "0",
                            ContractBillingType = "03",
                            BillingAmount = 4000,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 6,
                            ProductTypeCode = "6",
                            ChangeFeeDate = new DateTime(2012, 7, 15),
                            CreditTerm = 60

                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingOCC = "01",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "2020",
                            BillingTargetCode = "0000000001-002",
                            PaymentMethod = "0",
                            ContractBillingType = "06",
                            BillingAmount = null,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 6,
                            ProductTypeCode = "6",
                            ChangeFeeDate = new DateTime(2012, 7, 15),
                            CreditTerm = 60,


                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingOCC = "02",
                            BillingClientCode = "0000000002",
                            BillingOfficeCode = "2020",
                            BillingTargetCode = "0000000002-002",
                            PaymentMethod = "0",
                            ContractBillingType = "05",
                            BillingAmount = 4000,
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 6,
                            ProductTypeCode = "6",
                            ChangeFeeDate = new DateTime(2012, 7, 15),
                            CreditTerm = 60,

                        });

                        break;
                    case "15":

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "Q0000030015",
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            BillingTargetCode = "0000000001-001",
                            PaymentMethod = "0",
                            ContractBillingType = "11",
                            BillingCycle = 6,
                            CalculationDailyFee = "1",
                            ContractTiming = 1,
                            ProductTypeCode = "1",
                            CreditTerm = 60

                        });



                        break;
                    default:
                        break;


                }

                if (input_list.Count > 0)
                {
                    var result = handlerBilling.ManageBillingBasic(input_list);
                    res.ResultData = result;
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP011(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                // ManageBillingBasic

                List<doBillingTempDetail> input_list = new List<doBillingTempDetail>();

                switch (case_on)
                {
                    case "1":
                        input_list.Add(new doBillingTempDetail()
                        {
                            ContractBillingType = "06",
                            BillingDate = DateTime.Now,
                            BillingAmount = 1000.0000M,
                            PaymentMethod = "0",
                            BillingTiming = "0",

                        });


                        break;
                    case "2":
                        input_list.Add(new doBillingTempDetail()
                        {
                            ContractCode = "N0000000001",
                            SequenceNo = 1,
                            BillingOCC = "02",
                            ContractBillingType = "06",
                            BillingDate = DateTime.Now,
                            BillingAmount = 1000M,
                            PaymentMethod = "0",
                            ProductTypeCode = "2", // add new
                            BillingTiming = "0"

                        });


                        break;
                    case "3":
                        input_list.Add(new doBillingTempDetail()
                        {
                            ContractCode = "Q0000030015",
                            SequenceNo = 1,
                            BillingOCC = "01",
                            ContractBillingType = "11",
                            BillingDate = DateTime.Now,
                            BillingAmount = 1000,
                            PaymentMethod = "00",
                            ProductTypeCode = "1",
                            ProductCode = "005",
                            BillingTiming = "0"

                        });


                        break;
                    default:
                        break;


                }

                if (input_list.Count > 0)
                {
                    var result = handlerBilling.ManageBillingDetailByContract(input_list);
                    res.ResultData = result;
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP012(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                bool result = false;

                switch (case_on)
                {
                    case "1":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- StartService = 2/05/2012
                        //- AdjustDate = 31/05/2012

                        result = handlerBilling.ManageBillingBasicForStart("N0000000001", new DateTime(2012, 5, 2), new DateTime(2012, 5, 31));

                        break;
                    case "2":

                        //Parameters: 
                        //- ContractCode = MA0000510312
                        //- StartService = 2/05/2012
                        //- AdjustDate = NULL

                        result = handlerBilling.ManageBillingBasicForStart("MA0000510312", new DateTime(2012, 5, 2), null);

                        break;

                    default:
                        break;


                }


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP013(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                bool result = false;

                switch (case_on)
                {
                    case "1":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- ResumeDate = 01/07/2012

                        result = handlerBilling.ManageBillingBasicForResume("N0000000001", new DateTime(2012, 7, 1));

                        break;
                    case "2":

                        //Parameters: 
                        //- ContractCode = MA0000510312
                        //- ResumeDate = 01/07/2012

                        result = handlerBilling.ManageBillingBasicForResume("MA0000510312", new DateTime(2012, 7, 1));

                        break;

                    default:
                        break;


                }


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP014(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<AdjustOnNextPeriod> result_list = new List<AdjustOnNextPeriod>();
                AdjustOnNextPeriod result = new AdjustOnNextPeriod();

                switch (case_on)
                {
                    case "1":

                        //Parameters: 
                        //- ContractCode = MA0000510312
                        //- BillingOCC = 01
                        //- ChangeDate = 1/06/2012
                        //- MonthlyBillingAmount = 5000

                        result = handlerBilling.CalculateDifferenceMonthlyFee("N0000000001", "01", new DateTime(2012, 10, 18), 200, ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC);

                        break;

                    case "2":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- BillingOCC = 03
                        //- ChangeDate = 16/05/2012
                        //- MonthlyBillingAmount = 5000

                        result = handlerBilling.CalculateDifferenceMonthlyFee("N0000000001", "03", new DateTime(2012, 5, 16), 5000, ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC);

                        break;

                    case "3":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- BillingOCC = 01
                        //- ChangeDate = 1/06/2012
                        //- MonthlyBillingAmount = 5000

                        result = handlerBilling.CalculateDifferenceMonthlyFee("N0000000001", "01", new DateTime(2012, 6, 1), 5000, ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC);

                        break;

                    case "4":

                        //Parameters: 
                        //- ContractCode = MA0000510312
                        //- BillingOCC = 01
                        //- ChangeDate = 1/11/2012
                        //- MonthlyBillingAmount = 0

                        result = handlerBilling.CalculateDifferenceMonthlyFee("MA0000510312", "01", new DateTime(2012, 11, 1), 0, ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC);

                        break;

                    case "5":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- BillingOCC = 01
                        //- ChangeDate = 16/06/2012
                        //- MonthlyBillingAmount = 0

                        result = handlerBilling.CalculateDifferenceMonthlyFee("N0000000001", "01", new DateTime(2012, 6, 16), 0, ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC);

                        break;


                    default:
                        break;


                }

                result_list.Add(result);

                res.ResultData = result_list;



            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP015(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                bool result = false;

                switch (case_on)
                {
                    case "1":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- StopDate = 1/06/2012
                        //- StopFee = 0

                        result = handlerBilling.ManageBillingBasicForStop("N0000000001", new DateTime(2012, 6, 1), 0);

                        break;
                    case "2":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- StopDate = 16/06/2012
                        //- StopFee = 0

                        result = handlerBilling.ManageBillingBasicForStop("N0000000001", new DateTime(2012, 6, 16), 0);

                        break;

                    case "3":

                        //Parameters: 
                        //- ContractCode = MA0000510312
                        //- StopDate = 01/07/2012
                        //- StopFee = 1000


                        result = handlerBilling.ManageBillingBasicForStop("MA0000510312", new DateTime(2012, 7, 1), 1000);

                        break;
                    default:
                        break;

                }


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP016(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                bool result = false;

                switch (case_on)
                {
                    case "1":

                        //Parameters: 
                        //- ContractCode = N0000000001
                        //- CancelDate = 01/07/2012

                        result = handlerBilling.ManageBillingBasicForCancel("N0000000001", new DateTime(2012, 7, 1));

                        break;
                    default:
                        break;


                }


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP017(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                // ManageBillingBasic

                List<doBillingTempBasic> input_list = new List<doBillingTempBasic>();

                switch (case_on)
                {
                    case "1":
                        input_list.Add(new doBillingTempBasic()
                        {
                            BillingClientCode = "0000000001",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "01",
                            BillingAmount = 10000M,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 1,
                            ProductTypeCode = "2",
                            CreditTerm = 1


                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            BillingClientCode = "0000000002",
                            BillingOfficeCode = "1000",
                            PaymentMethod = "1",
                            ContractBillingType = "06",
                            BillingAmount = 1000M,
                            BillingCycle = 6,
                            CalculationDailyFee = "0",
                            ContractTiming = 1,
                            ProductTypeCode = "2",
                            CreditTerm = 1
                        });


                        break;
                    case "2":
                        input_list.Add(new doBillingTempBasic()
                         {
                             ContractCode = "N0000000001",
                             BillingOCC = "01",
                             BillingClientCode = "0000000005",
                             BillingOfficeCode = "1000",
                             ProductTypeCode = "2",
                             BillingTargetCode = "0000000005-001"

                         });

                        input_list.Add(new doBillingTempBasic()
                        {

                            ContractCode = "N0000000001",
                            BillingOCC = "03",
                            BillingClientCode = "0000000006",
                            ProductTypeCode = "2",
                            BillingOfficeCode = "1000"

                        });



                        break;
                    case "3":
                        input_list.Add(new doBillingTempBasic()
                        {

                            ContractCode = "MA0000510312",
                            BillingOCC = "01",
                            BillingClientCode = "0000000005",
                            ProductTypeCode = "6",
                            BillingOfficeCode = "2020",

                        });

                        input_list.Add(new doBillingTempBasic()
                        {
                            ContractCode = "MA0000510312",
                            BillingOCC = "03",
                            BillingClientCode = "0000000006",
                            ProductTypeCode = "6",
                            BillingOfficeCode = "2020"
                        });



                        break;
                    default:
                        break;


                }

                if (input_list.Count > 0)
                {
                    var result = handlerBilling.ManageBillingBasicForChangeNameAndAddress(input_list);
                    res.ResultData = result;
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP070(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<dtTbt_BillingTargetForView> result_list = new List<dtTbt_BillingTargetForView>();
                dtTbt_BillingTargetForView result = new dtTbt_BillingTargetForView();

                switch (case_on)
                {
                    case "1":

                        //Parameters: 
                        //- BillingTargetCode = NULL

                        result = handlerBilling.GetTbt_BillingTargetForViewData(null, MiscType.C_CUST_TYPE);

                        break;
                    case "2":

                        //Parameters: 
                        //- BillingTargetCode = 0000000022-001

                        result = handlerBilling.GetTbt_BillingTargetForViewData("0000000022-001", MiscType.C_CUST_TYPE);

                        break;
                    case "3":

                        //Parameters: 
                        //- BillingTargetCode = 0000000511-001

                        result = handlerBilling.GetTbt_BillingTargetForViewData("0000000511-001", MiscType.C_CUST_TYPE);

                        break;
                    default:
                        break;


                }

                result_list.Add(result);

                res.ResultData = result_list;


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP080(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<BillingBasicList> result_list = new List<BillingBasicList>();


                switch (case_on)
                {
                    case "1":

                        //Parameters: 
                        //- ContractCode = NULL

                        //result_list = handlerBilling.GetBillingBasicListData(null, MiscType.C_CUST_TYPE);

                        break;
                    case "2":

                        //Parameters: 
                        //- ContractCode = N0000000022

                        //result_list = handlerBilling.GetBillingBasicListData("N0000000022", MiscType.C_CUST_TYPE);

                        break;
                    case "3":

                        //Parameters: 
                        //-ContractCode = N0000000001

                        //result_list = handlerBilling.GetBillingBasicListData("N0000000001", MiscType.C_CUST_TYPE);

                        break;
                    default:
                        break;


                }



                res.ResultData = result_list;


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_BLP031(string case_on)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;


                tbt_Invoice result = null;
                List<tbt_Invoice> result_list = new List<tbt_Invoice>();

                tbt_Invoice invoice = new tbt_Invoice();
                List<tbt_BillingDetail> input_list = new List<tbt_BillingDetail>();

                switch (case_on)
                {
                    case "1":

                        // Invoice
                        invoice = new tbt_Invoice()
                        {
                            InvoiceNo = "201205A00074",
                            InvoiceOCC = 1,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            AutoTransferDate = null,
                            BillingTargetCode = "0000000406-001",
                            InvoiceAmount = 15000.0000M,
                            PaidAmountIncVat = null,
                            VatRate = null,
                            VatAmount = null,
                            WHTRate = null,
                            WHTAmount = null,
                            RegisteredWHTAmount = null,
                            InvoicePaymentStatus = "01",
                            IssueInvFlag = true,
                            FirstIssueInvDate = new DateTime(2012, 5, 15),
                            FirstIssueInvFlag = true,
                            PaymentMethod = "3",
                            CorrectReason = null,
                            RefOldInvoiceNo = null,
                            ReasonForFailure = null,
                            BillingTypeCode = "31"
                        };


                        // BillingDeatail
                        input_list.Add(new tbt_BillingDetail()
                        {
                            ContractCode = "N0000000012",
                            BillingOCC = "01",
                            BillingDetailNo = 1,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "01",
                            BillingAmount = 6300.0000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 4, 01),
                            BillingEndDate = new DateTime(2012, 6, 30),
                            PaymentMethod = "3",
                            PaymentStatus = "01",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null
                        });

                        input_list.Add(new tbt_BillingDetail()
                        {

                            ContractCode = "SG0000000179",
                            BillingOCC = "01",
                            BillingDetailNo = 1,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "31",
                            BillingAmount = 30000.0000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 4, 1),
                            BillingEndDate = new DateTime(2012, 6, 30),
                            PaymentMethod = "3",
                            PaymentStatus = "01",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null
                        });

                        result = handlerBilling.ManageInvoiceByCommand(invoice, input_list, true);

                        break;
                    case "2":

                        // Invoice
                        invoice = new tbt_Invoice()
                        {
                            InvoiceNo = "201205A00074",
                            InvoiceOCC = 1,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            AutoTransferDate = null,
                            BillingTargetCode = "0000000406-001",
                            InvoiceAmount = 41300.0000M,
                            PaidAmountIncVat = null,
                            VatRate = null,
                            VatAmount = null,
                            WHTRate = null,
                            WHTAmount = null,
                            RegisteredWHTAmount = null,
                            InvoicePaymentStatus = "01",
                            IssueInvFlag = true,
                            FirstIssueInvDate = new DateTime(2012, 5, 15),
                            FirstIssueInvFlag = true,
                            PaymentMethod = "3",
                            CorrectReason = null,
                            RefOldInvoiceNo = null,
                            ReasonForFailure = null,
                            BillingTypeCode = "31"
                        };


                        // BillingDeatail
                        input_list.Add(new tbt_BillingDetail()
                        {

                            ContractCode = "N0000000012",
                            BillingOCC = "01",
                            BillingDetailNo = 1,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "01",
                            BillingAmount = 6300.0000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 4, 1),
                            BillingEndDate = new DateTime(2012, 6, 30),
                            PaymentMethod = "3",
                            PaymentStatus = "01",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null
                        });

                        input_list.Add(new tbt_BillingDetail()
                        {

                            ContractCode = "N0000000013",
                            BillingOCC = "01",
                            BillingDetailNo = 1,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "01",
                            BillingAmount = 35000.0000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 5, 1),
                            BillingEndDate = new DateTime(2012, 5, 31),
                            PaymentMethod = "0",
                            PaymentStatus = "01",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null

                        });


                        result = handlerBilling.ManageInvoiceByCommand(invoice, input_list, true);

                        break;
                    case "3":
                        // Invoice
                        invoice = new tbt_Invoice()
                        {
                            InvoiceNo = null,
                            InvoiceOCC = 0,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            AutoTransferDate = null,
                            BillingTargetCode = "0000000001-001",
                            InvoiceAmount = 15000.0000M,
                            PaidAmountIncVat = null,
                            VatRate = null,
                            VatAmount = null,
                            WHTRate = null,
                            WHTAmount = null,
                            RegisteredWHTAmount = null,
                            InvoicePaymentStatus = "01",
                            IssueInvFlag = true,
                            FirstIssueInvDate = new DateTime(2012, 5, 15),
                            FirstIssueInvFlag = true,
                            PaymentMethod = "0",
                            CorrectReason = null,
                            RefOldInvoiceNo = null,
                            ReasonForFailure = null,
                            BillingTypeCode = "47"
                        };


                        // BillingDeatail
                        input_list.Add(new tbt_BillingDetail()
                        {
                            ContractCode = "Q0000030015",
                            BillingOCC = "01",
                            BillingDetailNo = 0,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "47",
                            BillingAmount = 15000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 5, 15),
                            BillingEndDate = null,
                            PaymentMethod = "0",
                            PaymentStatus = "00",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null

                        });


                        result = handlerBilling.ManageInvoiceByCommand(invoice, input_list, true);

                        break;
                    case "4":
                        // Invoice
                        invoice = new tbt_Invoice()
                        {
                            InvoiceNo = "201205A00003",
                            InvoiceOCC = 1,
                            IssueInvDate = new DateTime(2012, 05, 15),
                            AutoTransferDate = null,
                            BillingTargetCode = "0000000406-001",
                            InvoiceAmount = 41300.0000M,
                            PaidAmountIncVat = null,
                            VatRate = null,
                            VatAmount = null,
                            WHTRate = null,
                            WHTAmount = null,
                            RegisteredWHTAmount = null,
                            InvoicePaymentStatus = "01",
                            IssueInvFlag = true,
                            FirstIssueInvDate = new DateTime(2012, 5, 15),
                            FirstIssueInvFlag = true,
                            PaymentMethod = "3",
                            CorrectReason = null,
                            RefOldInvoiceNo = null,
                            ReasonForFailure = null,
                            BillingTypeCode = "01"
                        };


                        // BillingDeatail
                        input_list.Add(new tbt_BillingDetail()
                        {
                            ContractCode = "N0000000012",
                            BillingOCC = "01",
                            BillingDetailNo = 1,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "01",
                            BillingAmount = 6300.0000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 4, 1),
                            BillingEndDate = new DateTime(2012, 6, 30),
                            PaymentMethod = "3",
                            PaymentStatus = "01",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null

                        });

                        input_list.Add(new tbt_BillingDetail()
                        {
                            ContractCode = "N0000000013",
                            BillingOCC = "01",
                            BillingDetailNo = 1,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "01",
                            BillingAmount = 35000.0000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 5, 1),
                            BillingEndDate = new DateTime(2012, 5, 31),
                            PaymentMethod = "3",
                            PaymentStatus = "01",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null


                        });



                        result = handlerBilling.ManageInvoiceByCommand(invoice, input_list, true);

                        break;
                    case "5":
                        // Invoice
                        invoice = new tbt_Invoice()
                        {
                            InvoiceNo = null,
                            InvoiceOCC = 1,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            AutoTransferDate = null,
                            BillingTargetCode = "0000000415-001",
                            InvoiceAmount = 30000.0000M,
                            PaidAmountIncVat = null,
                            VatRate = null,
                            VatAmount = null,
                            WHTRate = null,
                            WHTAmount = null,
                            RegisteredWHTAmount = null,
                            InvoicePaymentStatus = "01",
                            IssueInvFlag = true,
                            FirstIssueInvDate = new DateTime(2012, 5, 15),
                            FirstIssueInvFlag = true,
                            PaymentMethod = "3",
                            CorrectReason = null,
                            RefOldInvoiceNo = null,
                            ReasonForFailure = null,
                            BillingTypeCode = "21"

                        };


                        // BillingDeatail
                        input_list.Add(new tbt_BillingDetail()
                        {

                            ContractCode = "SG0000000179",
                            BillingOCC = "01",
                            BillingDetailNo = 0,
                            InvoiceNo = null,
                            InvoiceOCC = null,
                            IssueInvDate = new DateTime(2012, 5, 15),
                            IssueInvFlag = true,
                            BillingTypeCode = "21",
                            BillingAmount = 30000.0000M,
                            AdjustBillingAmount = null,
                            BillingStartDate = new DateTime(2012, 5, 15),
                            BillingEndDate = null,
                            PaymentMethod = "3",
                            PaymentStatus = "01",
                            AutoTransferDate = null,
                            FirstFeeFlag = null,
                            DelayedMonth = null

                        });


                        result = handlerBilling.ManageInvoiceByCommand(invoice, input_list, true);

                        break;
                    default:
                        break;


                }

                result_list.Add(result);
                res.ResultData = result_list;

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }
    }
}
