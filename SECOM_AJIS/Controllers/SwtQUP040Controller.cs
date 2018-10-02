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
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Controllers
{
    public class SwtQUP040Controller : SwtCommonController
    {
        //
        // GET: /SwtQUP040/

        public string index()
        {
            //Using in write log process
            CommonUtil.dsTransData = new dsTransDataModel();
            CommonUtil.dsTransData.dtUserData = new UserDataDo();
            CommonUtil.dsTransData.dtOperationData = new OperationDataDo();
            CommonUtil.dsTransData.dtTransHeader = new TransHeaderDo();
            
            //Login user = 510729
            //Process datetime = 2011/12/03  06:15:00 PM
            CommonUtil.dsTransData.dtUserData.EmpNo = "510729";
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 12, 3, 18, 15, 00);
            CommonUtil.dsTransData.dtTransHeader.ScreenID = "QUP040"; 

            List<string> lst = new List<string>();
            //lst.Add(Case1());
            //lst.Add(Case2());
            //lst.Add(Case3());
            //lst.Add(Case4());
            //lst.Add(Case5());
            //lst.Add(Case6());
            //lst.Add(Case7());
            lst.Add(Case8());
            //lst.Add(Case9());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose   : Mandatory check1  (QuotationHandler.GenerateQuotation)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = NULL
        ///                 - InstallationFee = NULL
        ///                 - InstallationSlipNo = NULL
        ///                 - InstallationEngineerEmpNo = NULL
        ///                 - ApproveNo1 = NULL
        ///                 - ApproveNo2 = NULL
        ///             dtInstrumentDetails[] = Empty list
        ///Expected  : MSG0007: These field was required: ContractCode, InstallationFee, dtInstrumentDetails[].
        ///</summary>
        public string Case1()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = null;
            doUpdate.dtHeader.InstallationFee = null;
            doUpdate.dtHeader.InstallationSlipNo = null;
            doUpdate.dtHeader.InstallationEngineerEmpNo = null;
            doUpdate.dtHeader.ApproveNo1 = null;
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();

            string expected = "MSG0007";
            string actual;

            try
            {
                actual = target.GenerateQuotation(doUpdate);
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
        ///Purpose   : Mandatory check2 (QuotationHandler.GenerateQuotation)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = N0000000346
        ///                 - InstallationFee = 100000.75
        ///                 - InstallationSlipNo = NULL
        ///                 - InstallationEngineerEmpNo = NULL
        ///                 - ApproveNo1 = NULL
        ///                 - ApproveNo2 = NULL
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = AC-A1030TH
        ///                 - InstallQty = 5
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///                - dtInstrumentDetails[1] 
        ///                 - InstrumentCode = NULL
        ///                 - InstallQty = NULL
        ///                 - AcmAddQty = 10
        ///                 - AcmRemoveQty = NULL
        ///Expected  : MSG2013: Line 1, these field was required: InstrumentCode, InstallQty.
        ///</summary>
        public string Case2()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "N0000000346";
            doUpdate.dtHeader.InstallationFee = 100000.75M;
            doUpdate.dtHeader.InstallationSlipNo = null;
            doUpdate.dtHeader.InstallationEngineerEmpNo = null;
            doUpdate.dtHeader.ApproveNo1 = null;
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "AC-A1030TH";
            dt1.InstallQty = 5;
            dt1.AcmAddQty = null;
            dt1.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt1);
            //detail[1]
            dtInstrumentDetails dt2 = new dtInstrumentDetails();
            dt2.InstrumentCode = null;
            dt2.InstallQty = null;
            dt2.AcmAddQty = 10;
            dt2.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt2);

            string expected = "MSG2013";
            string actual;

            try
            {
                actual = target.GenerateQuotation(doUpdate);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Get contract data (How does the system perform if it cannot get contract data)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = Q0000000135
        ///                 - InstallationFee = 250000.00
        ///                 - InstallationSlipNo = NULL
        ///                 - InstallationEngineerEmpNo = NULL
        ///                 - ApproveNo1 = NULL
        ///                 - ApproveNo2 = NULL
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = AC-A1030TH
        ///                 - InstallQty = 5
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///Expected  : MSG2085: Contract data not found, Q0000135.
        ///</summary>
        public string Case3()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "Q0000000135";
            doUpdate.dtHeader.InstallationFee = 250000.00M;
            doUpdate.dtHeader.InstallationSlipNo = null;
            doUpdate.dtHeader.InstallationEngineerEmpNo = null;
            doUpdate.dtHeader.ApproveNo1 = null;
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "AC-A1030TH";
            dt1.InstallQty = 5;
            dt1.AcmAddQty = null;
            dt1.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt1);

            string expected = "MSG2085";
            string actual;

            try
            {
                actual = target.GenerateQuotation(doUpdate);
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
        ///Purpose   : Get contract data (How does the system perform if it cannot get contract data)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = N0000000381
        ///                 - InstallationFee = 250000.00
        ///                 - InstallationSlipNo = NULL
        ///                 - InstallationEngineerEmpNo = NULL
        ///                 - ApproveNo1 = NULL
        ///                 - ApproveNo2 = NULL
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = AC-A1030TH
        ///                 - InstallQty = 5
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///Expected  : MSG2014: Cannot generate quotation for not-alarm contract.
        ///</summary>
        public string Case4()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "N0000000381";
            doUpdate.dtHeader.InstallationFee = 250000.00M;
            doUpdate.dtHeader.InstallationSlipNo = null;
            doUpdate.dtHeader.InstallationEngineerEmpNo = null;
            doUpdate.dtHeader.ApproveNo1 = null;
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "AC-A1030TH";
            dt1.InstallQty = 5;
            dt1.AcmAddQty = null;
            dt1.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt1);

            string expected = "MSG2014";
            string actual;

            try
            {
                actual = target.GenerateQuotation(doUpdate);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Get contract data (How does the system perform if an invalid instrument is specified, master data not found.)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = N0000000346
        ///                 - InstallationFee = 250000.00
        ///                 - InstallationSlipNo = NULL
        ///                 - InstallationEngineerEmpNo = NULL
        ///                 - ApproveNo1 = NULL
        ///                 - ApproveNo2 = NULL
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = AC-A1030TH
        ///                 - InstallQty = 5
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///                - dtInstrumentDetails[1] 
        ///                 - InstrumentCode = GT-I9100T
        ///                 - InstallQty = 20
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///Expected  : MSG2015: Cannot generate quotation. Master data of the instrument code, GT-I9100T, not found.
        ///</summary>
        public string Case5()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "N0000000346";
            doUpdate.dtHeader.InstallationFee = 250000.00M;
            doUpdate.dtHeader.InstallationSlipNo = null;
            doUpdate.dtHeader.InstallationEngineerEmpNo = null;
            doUpdate.dtHeader.ApproveNo1 = null;
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "AC-A1030TH";
            dt1.InstallQty = 5;
            dt1.AcmAddQty = null;
            dt1.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt1);
            //detail[1]
            dtInstrumentDetails dt2 = new dtInstrumentDetails();
            dt2.InstrumentCode = "GT-I9100T";
            dt2.InstallQty = 20;
            dt2.AcmAddQty = null;
            dt2.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt2);

            string expected = "MSG2015";
            string actual;

            try
            {
                actual = target.GenerateQuotation(doUpdate);
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
        ///Purpose   : Get contract data (How does the system perform if an invalid instrument is specified, master data not found.)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = N0000000346
        ///                 - InstallationFee = 250000.00
        ///                 - InstallationSlipNo = NULL
        ///                 - InstallationEngineerEmpNo = NULL
        ///                 - ApproveNo1 = NULL
        ///                 - ApproveNo2 = NULL
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = AC-A1030TH
        ///                 - InstallQty = 5
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///                - dtInstrumentDetails[1] 
        ///                 - InstrumentCode = U100i
        ///                 - InstallQty = 20
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///Expected  : MSG2016: Cannot generate quotation. Rental unit price of the instrument code, U100i, not found.
        ///</summary>
        public string Case6()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "N0000000346";
            doUpdate.dtHeader.InstallationFee = 250000.00M;
            doUpdate.dtHeader.InstallationSlipNo = null;
            doUpdate.dtHeader.InstallationEngineerEmpNo = null;
            doUpdate.dtHeader.ApproveNo1 = null;
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "AC-A1030TH";
            dt1.InstallQty = 5;
            dt1.AcmAddQty = null;
            dt1.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt1);
            //detail[1]
            dtInstrumentDetails dt2 = new dtInstrumentDetails();
            dt2.InstrumentCode = "U100i";
            dt2.InstallQty = 20;
            dt2.AcmAddQty = null;
            dt2.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt2);

            string expected = "MSG2016";
            string actual;

            try
            {
                actual = target.GenerateQuotation(doUpdate);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate quotation (quotation target already exists)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = N0000000346
        ///                 - InstallationFee = 100000.00
        ///                 - InstallationSlipNo = 0001001201004008
        ///                 - InstallationEngineerEmpNo = 540902
        ///                 - ApproveNo1 = AR-000000005001
        ///                 - ApproveNo2 = AR-000000005008
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = AC-A1030TH
        ///                 - InstallQty = 9999
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = 9000
        ///                - dtInstrumentDetails[1] 
        ///                 - InstrumentCode = GEA-CE4-D36P-IP
        ///                 - InstallQty = 20
        ///                 - AcmAddQty = 5
        ///                 - AcmRemoveQty = NULL
        ///                - dtInstrumentDetails[2] 
        ///                 - InstrumentCode = JKT-03120
        ///                 - InstallQty = 56
        ///                 - AcmAddQty = NULL
        ///                 - AcmRemoveQty = NULL
        ///                - dtInstrumentDetails[3] 
        ///                 - InstrumentCode = SP40S
        ///                 - InstallQty = 0
        ///                 - AcmAddQty = 7
        ///                 - AcmRemoveQty = NULL
        ///                - dtInstrumentDetails[4] 
        ///                 - InstrumentCode = HSG-IP65BIR
        ///                 - InstallQty = 40
        ///                 - AcmAddQty = 7
        ///                 - AcmRemoveQty = 3
        ///Expected  : See expectation test case7
        ///</summary>
        public string Case7()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "N0000000346";
            doUpdate.dtHeader.InstallationFee = 100000.00M;
            doUpdate.dtHeader.InstallationSlipNo = "0001001201004008";
            doUpdate.dtHeader.InstallationEngineerEmpNo = "540902";
            doUpdate.dtHeader.ApproveNo1 = "AR-000000005001";
            doUpdate.dtHeader.ApproveNo2 = "AR-000000005008";
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "AC-A1030TH";
            dt1.InstallQty = 9999;
            dt1.AcmAddQty = null;
            dt1.AcmRemoveQty = 9000;
            doUpdate.dtInstrumentDetails.Add(dt1);
            //detail[1]
            dtInstrumentDetails dt2 = new dtInstrumentDetails();
            dt2.InstrumentCode = "GEA-CE4-D36P-IP";
            dt2.InstallQty = 20;
            dt2.AcmAddQty = 5;
            dt2.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt2);
            //detail[2]
            dtInstrumentDetails dt3 = new dtInstrumentDetails();
            dt3.InstrumentCode = "JKT-03120";
            dt3.InstallQty = 56;
            dt3.AcmAddQty = null;
            dt3.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt3);
            //detail[3]
            dtInstrumentDetails dt4 = new dtInstrumentDetails();
            dt4.InstrumentCode = "SP40S";
            dt4.InstallQty = 0;
            dt4.AcmAddQty = 7;
            dt4.AcmRemoveQty = null;
            doUpdate.dtInstrumentDetails.Add(dt4);
            //detail[4]
            dtInstrumentDetails dt5 = new dtInstrumentDetails();
            dt5.InstrumentCode = "HSG-IP65BIR";
            dt5.InstallQty = 40;
            dt5.AcmAddQty = 7;
            dt5.AcmRemoveQty = 3;
            doUpdate.dtInstrumentDetails.Add(dt5);

            string expected = "AB";
            string actual;

            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case7";
                actual = target.GenerateQuotation(doUpdate);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate quotation (quotation target already exists)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = N0000000381
        ///                 - InstallationFee = 472000.75
        ///                 - InstallationSlipNo = 0001001201005009
        ///                 - InstallationEngineerEmpNo = 490441
        ///                 - ApproveNo1 = AR-000000005009
        ///                 - ApproveNo2 = null
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = IQ-600NB
        ///                 - InstallQty = 25
        ///                 - AcmAddQty = 3
        ///                 - AcmRemoveQty = 0
        ///                - dtInstrumentDetails[1] 
        ///                 - InstrumentCode = JKT-03120
        ///                 - InstallQty = 12
        ///                 - AcmAddQty = 0
        ///                 - AcmRemoveQty = 2
        ///                - dtInstrumentDetails[2] 
        ///                 - InstrumentCode = SMK-2000A
        ///                 - InstallQty = 10
        ///                 - AcmAddQty = 0
        ///                 - AcmRemoveQty = 0
        ///                - dtInstrumentDetails[3] 
        ///                 - InstrumentCode = UL135S
        ///                 - InstallQty = 0
        ///                 - AcmAddQty = 6
        ///                 - AcmRemoveQty = 0
        ///                - dtInstrumentDetails[4] 
        ///                 - InstrumentCode = YV10X5B-SA2-PRO
        ///                 - InstallQty = 1
        ///                 - AcmAddQty = 17
        ///                 - AcmRemoveQty = 4
        ///Expected  : See expectation test case8
        ///</summary>
        public string Case8()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "N0000000381";
            doUpdate.dtHeader.InstallationFee = 472000.75M;
            doUpdate.dtHeader.InstallationSlipNo = "0001001201005009";
            doUpdate.dtHeader.InstallationEngineerEmpNo = "490441";
            doUpdate.dtHeader.ApproveNo1 = "AR-000000005009";
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "IQ-600NB";
            dt1.InstallQty = 25;
            dt1.AcmAddQty = 3;
            dt1.AcmRemoveQty = 0;
            doUpdate.dtInstrumentDetails.Add(dt1);
            //detail[1]
            dtInstrumentDetails dt2 = new dtInstrumentDetails();
            dt2.InstrumentCode = "JKT-03120";
            dt2.InstallQty = 12;
            dt2.AcmAddQty = 0;
            dt2.AcmRemoveQty = 2;
            doUpdate.dtInstrumentDetails.Add(dt2);
            //detail[2]
            dtInstrumentDetails dt3 = new dtInstrumentDetails();
            dt3.InstrumentCode = "SMK-2000A";
            dt3.InstallQty = 10;
            dt3.AcmAddQty = 0;
            dt3.AcmRemoveQty = 0;
            doUpdate.dtInstrumentDetails.Add(dt3);
            //detail[3]
            dtInstrumentDetails dt4 = new dtInstrumentDetails();
            dt4.InstrumentCode = "UL135S";
            dt4.InstallQty = 0;
            dt4.AcmAddQty = 6;
            dt4.AcmRemoveQty = 0;
            doUpdate.dtInstrumentDetails.Add(dt4);
            //detail[4]
            dtInstrumentDetails dt5 = new dtInstrumentDetails();
            dt5.InstrumentCode = "YV10X5B-SA2-PRO";
            dt5.InstallQty = 1;
            dt5.AcmAddQty = 17;
            dt5.AcmRemoveQty = 4;
            doUpdate.dtInstrumentDetails.Add(dt5);

            string expected = "AA";
            string actual;

            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case8";
                actual = target.GenerateQuotation(doUpdate);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate quotation (quotation target already exists)
        ///Parameters: dsGenerateQuotationData
        ///             dtHeader[0]
        ///                 - ContractCode = N0000000385
        ///                 - InstallationFee = 472000.75
        ///                 - InstallationSlipNo = 0001001201005009
        ///                 - InstallationEngineerEmpNo = 490441
        ///                 - ApproveNo1 = AR-000000005009
        ///                 - ApproveNo2 = null
        ///             dtInstrumentDetails[] 
        ///                - dtInstrumentDetails[0] 
        ///                 - InstrumentCode  = IQ-600NB
        ///                 - InstallQty = 25
        ///                 - AcmAddQty = 3
        ///                 - AcmRemoveQty = 0
        ///                - dtInstrumentDetails[1] 
        ///                 - InstrumentCode = JKT-03120
        ///                 - InstallQty = 12
        ///                 - AcmAddQty = 0
        ///                 - AcmRemoveQty = 2
        ///                - dtInstrumentDetails[2] 
        ///                 - InstrumentCode = SMK-2000A
        ///                 - InstallQty = 10
        ///                 - AcmAddQty = 0
        ///                 - AcmRemoveQty = 0
        ///                - dtInstrumentDetails[3] 
        ///                 - InstrumentCode = UL135S
        ///                 - InstallQty = 0
        ///                 - AcmAddQty = 6
        ///                 - AcmRemoveQty = 0
        ///                - dtInstrumentDetails[4] 
        ///                 - InstrumentCode = YV10X5B-SA2-PRO
        ///                 - InstallQty = 1
        ///                 - AcmAddQty = 17
        ///                 - AcmRemoveQty = 4
        ///Expected  : See expectation test case9
        ///</summary>
        public string Case9()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dsGenerateData doUpdate = new dsGenerateData();
            //header
            doUpdate.dtHeader = new dtHeader();
            doUpdate.dtHeader.ContractCode = "N0000000385";
            doUpdate.dtHeader.InstallationFee = 472000.75M;
            doUpdate.dtHeader.InstallationSlipNo = "0001001201005009";
            doUpdate.dtHeader.InstallationEngineerEmpNo = "490441";
            doUpdate.dtHeader.ApproveNo1 = "AR-000000005009";
            doUpdate.dtHeader.ApproveNo2 = null;
            //detail
            doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();
            //detail[0]
            dtInstrumentDetails dt1 = new dtInstrumentDetails();
            dt1.InstrumentCode = "IQ-600NB";
            dt1.InstallQty = 25;
            dt1.AcmAddQty = 3;
            dt1.AcmRemoveQty = 0;
            doUpdate.dtInstrumentDetails.Add(dt1);
            //detail[1]
            dtInstrumentDetails dt2 = new dtInstrumentDetails();
            dt2.InstrumentCode = "JKT-03120";
            dt2.InstallQty = 12;
            dt2.AcmAddQty = 0;
            dt2.AcmRemoveQty = 2;
            doUpdate.dtInstrumentDetails.Add(dt2);
            //detail[2]
            dtInstrumentDetails dt3 = new dtInstrumentDetails();
            dt3.InstrumentCode = "SMK-2000A";
            dt3.InstallQty = 10;
            dt3.AcmAddQty = 0;
            dt3.AcmRemoveQty = 0;
            doUpdate.dtInstrumentDetails.Add(dt3);
            //detail[3]
            dtInstrumentDetails dt4 = new dtInstrumentDetails();
            dt4.InstrumentCode = "UL135S";
            dt4.InstallQty = 0;
            dt4.AcmAddQty = 6;
            dt4.AcmRemoveQty = 0;
            doUpdate.dtInstrumentDetails.Add(dt4);
            //detail[4]
            dtInstrumentDetails dt5 = new dtInstrumentDetails();
            dt5.InstrumentCode = "YV10X5B-SA2-PRO";
            dt5.InstallQty = 1;
            dt5.AcmAddQty = 17;
            dt5.AcmRemoveQty = 4;
            doUpdate.dtInstrumentDetails.Add(dt5);

            string expected = "AA";
            string actual;

            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case9";
                actual = target.GenerateQuotation(doUpdate);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_String(expected, actual));
        }

    }
}

