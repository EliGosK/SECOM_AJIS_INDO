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
    public class SwtCMP010Controller : SwtCommonController {
        //
        // GET: /SwtCMP010/

        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());
            lst.Add(Case5());
            lst.Add(Case6());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Check mandatory filelds.
        ///     
        ///Parameters:
        ///     doDocumentDataGenerate:
        ///         DocumentNo: NULL
        ///         DocumentCode: ""
        ///         DocumentData: ""
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: DocumentNo."
        ///</summary>
        public string Case1() {
            IDocumentHandler target = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            doDocumentDataGenerate param = new doDocumentDataGenerate();
            param.DocumentNo = null;
            param.DocumentCode = "";
            param.DocumentData = "";
            string expected = "MSG0007";
            string actual = null;

            try {
                target.GenerateDocument(param);
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
        ///     doDocumentDataGenerate:
        ///         DocumentNo: ""
        ///         DocumentCode: NULL
        ///         DocumentData: ""
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: DocumentCode."
        ///</summary>
        public string Case2() {
            IDocumentHandler target = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            doDocumentDataGenerate param = new doDocumentDataGenerate();
            param.DocumentNo = "";
            param.DocumentCode = null;
            param.DocumentData = "";
            string expected = "MSG0007";
            string actual = null;

            try {
                target.GenerateDocument(param);
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
        ///     doDocumentDataGenerate:
        ///         DocumentNo: ""
        ///         DocumentCode: ""
        ///         DocumentData: NULL
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: DocumentData."
        ///</summary>
        public string Case3() {
            IDocumentHandler target = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            doDocumentDataGenerate param = new doDocumentDataGenerate();
            param.DocumentNo = "";
            param.DocumentCode = "";
            param.DocumentData = null;
            string expected = "MSG0007";
            string actual = null;

            try {
                target.GenerateDocument(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Check report template file path is not blank.
        ///     
        ///Parameters:
        ///     doDocumentDataGenerate:
        ///         DocumentNo: "000001"
        ///         DocumentCode: "CMR010"
        ///         DocumentData: ""
        ///         
        ///Expected:
        ///     MSG0064: "Cannot find report path for document type CMR010 not found."
        ///     Error Windows log:
        ///         Log type: Error
        ///         Log message: Report has not found.
        ///</summary>
        public string Case4() {
            IDocumentHandler target = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            doDocumentDataGenerate param = new doDocumentDataGenerate();
            param.DocumentNo = "000001";
            param.DocumentCode = "CMR010";
            param.DocumentData = "";
            string expected = "MSG0064";
            string actual = null;

            try {
                target.GenerateDocument(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Check report template file path is existing but not valid.
        ///     
        ///Parameters:
        ///     doDocumentDataGenerate:
        ///         DocumentNo: "000001"
        ///         DocumentCode: "IVR192"
        ///         DocumentData: ""
        ///         
        ///Expected:
        ///     MSG0065: "Cannot find <an invalid file path>. Please contact administrator."
        ///     Error Windows log:
        ///         Log type: Error
        ///         Log message: Report has not found.
        ///</summary>
        public string Case5() {
            IDocumentHandler target = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            doDocumentDataGenerate param = new doDocumentDataGenerate();
            param.DocumentNo = "000001";
            param.DocumentCode = "IVR192";
            param.DocumentData = "";
            string expected = "MSG0065";
            string actual = null;

            try {
                target.GenerateDocument(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Check report template file path is existing but not valid.
        ///     
        ///Parameters:
        ///     refer to "SECOM-AJIS-STC.CMP010-Process of generating document" tab 'Test Data'
        ///         
        ///Expected:
        ///     refer to "SECOM-AJIS-STC.CMP010-Process of generating document" tab 'Expected result'
        ///</summary>
        public string Case6() {
            IDocumentHandler target = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            doDocumentDataGenerate param = new doDocumentDataGenerate();

            string expected = null;
            string actual = null;

            try {
                // TODO Akat K. ContactOffice is wrong name
                DateTime start = new DateTime(2011, 1, 1);
                param.DocumentCode = "BLR010"; param.DocumentNo = "000001"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0001"; param.OtherKey.QuotationTargetCode = "FN0000000013"; param.OtherKey.Alphabet = "01"; param.OtherKey.ProjectCode = "P0000015"; param.OtherKey.BillingTargetCode = "1230000014-001"; param.OtherKey.InstrumentCode = "ICTest01"; param.OtherKey.ContractOffice = "0001"; param.OtherKey.OperationOffice = "0002"; param.OtherKey.BillingOffice = "0003"; param.OtherKey.InstallationSlipIssueOffice = "0005"; param.OtherKey.MonthYear = start;
                target.GenerateDocument(param);
                param.DocumentCode = "BLR020"; param.DocumentNo = "000002"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0002"; param.OtherKey.QuotationTargetCode = "FN0000000022"; param.OtherKey.Alphabet = "02"; param.OtherKey.ProjectCode = "P0000023"; param.OtherKey.BillingTargetCode = "1230000026-001"; param.OtherKey.InstrumentCode = "ICTest02"; param.OtherKey.ContractOffice = "0002"; param.OtherKey.OperationOffice = "0003"; param.OtherKey.BillingOffice = "0004"; param.OtherKey.InstallationSlipIssueOffice = "0006"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR010"; param.DocumentNo = "000003"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0003"; param.OtherKey.QuotationTargetCode = "FN0000000034"; param.OtherKey.Alphabet = "03"; param.OtherKey.ProjectCode = "P0000036"; param.OtherKey.BillingTargetCode = "1230000031-001"; param.OtherKey.InstrumentCode = "ICTest03"; param.OtherKey.ContractOffice = "0003"; param.OtherKey.OperationOffice = "0004"; param.OtherKey.BillingOffice = "0005"; param.OtherKey.InstallationSlipIssueOffice = "0007"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR011"; param.DocumentNo = "000004"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0004"; param.OtherKey.QuotationTargetCode = "FN0000000041"; param.OtherKey.Alphabet = "04"; param.OtherKey.ProjectCode = "P0000049"; param.OtherKey.BillingTargetCode = "1230000046-001"; param.OtherKey.InstrumentCode = "ICTest04"; param.OtherKey.ContractOffice = "0004"; param.OtherKey.OperationOffice = "0005"; param.OtherKey.BillingOffice = "0006"; param.OtherKey.InstallationSlipIssueOffice = "0008"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR020"; param.DocumentNo = "000005"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0005"; param.OtherKey.QuotationTargetCode = "FN0000000059"; param.OtherKey.Alphabet = "05"; param.OtherKey.ProjectCode = "P0000055"; param.OtherKey.BillingTargetCode = "1230000051-001"; param.OtherKey.InstrumentCode = "ICTest05"; param.OtherKey.ContractOffice = "0005"; param.OtherKey.OperationOffice = "0006"; param.OtherKey.BillingOffice = "0007"; param.OtherKey.InstallationSlipIssueOffice = "0009"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR030"; param.DocumentNo = "000006"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0006"; param.OtherKey.QuotationTargetCode = "FN0000000060"; param.OtherKey.Alphabet = "06"; param.OtherKey.ProjectCode = "P0000067"; param.OtherKey.BillingTargetCode = "1230000060-001"; param.OtherKey.InstrumentCode = "ICTest06"; param.OtherKey.ContractOffice = "0006"; param.OtherKey.OperationOffice = "0007"; param.OtherKey.BillingOffice = "0008"; param.OtherKey.InstallationSlipIssueOffice = "0010"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR050"; param.DocumentNo = "000007"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0007"; param.OtherKey.QuotationTargetCode = "FN0000000073"; param.OtherKey.Alphabet = "07"; param.OtherKey.ProjectCode = "P0000076"; param.OtherKey.BillingTargetCode = "1230000077-001"; param.OtherKey.InstrumentCode = "ICTest07"; param.OtherKey.ContractOffice = "0007"; param.OtherKey.OperationOffice = "0008"; param.OtherKey.BillingOffice = "0009"; param.OtherKey.InstallationSlipIssueOffice = "0011"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR060"; param.DocumentNo = "000008"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0008"; param.OtherKey.QuotationTargetCode = "FN0000000084"; param.OtherKey.Alphabet = "08"; param.OtherKey.ProjectCode = "P0000081"; param.OtherKey.BillingTargetCode = "1230000083-001"; param.OtherKey.InstrumentCode = "ICTest08"; param.OtherKey.ContractOffice = "0008"; param.OtherKey.OperationOffice = "0009"; param.OtherKey.BillingOffice = "0010"; param.OtherKey.InstallationSlipIssueOffice = "0012"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR070"; param.DocumentNo = "000009"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0009"; param.OtherKey.QuotationTargetCode = "FN0000000091"; param.OtherKey.Alphabet = "09"; param.OtherKey.ProjectCode = "P0000090"; param.OtherKey.BillingTargetCode = "1230000091-001"; param.OtherKey.InstrumentCode = "ICTest09"; param.OtherKey.ContractOffice = "0009"; param.OtherKey.OperationOffice = "0010"; param.OtherKey.BillingOffice = "0011"; param.OtherKey.InstallationSlipIssueOffice = "0013"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR080"; param.DocumentNo = "000010"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0010"; param.OtherKey.QuotationTargetCode = "FN0000000108"; param.OtherKey.Alphabet = "10"; param.OtherKey.ProjectCode = "P0000105"; param.OtherKey.BillingTargetCode = "1230000101-001"; param.OtherKey.InstrumentCode = "ICTest10"; param.OtherKey.ContractOffice = "0010"; param.OtherKey.OperationOffice = "0011"; param.OtherKey.BillingOffice = "0012"; param.OtherKey.InstallationSlipIssueOffice = "0014"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR100"; param.DocumentNo = "000011"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0011"; param.OtherKey.QuotationTargetCode = "FN0000000117"; param.OtherKey.Alphabet = "11"; param.OtherKey.ProjectCode = "P0000115"; param.OtherKey.BillingTargetCode = "1230000114-001"; param.OtherKey.InstrumentCode = "ICTest11"; param.OtherKey.ContractOffice = "0011"; param.OtherKey.OperationOffice = "0012"; param.OtherKey.BillingOffice = "0013"; param.OtherKey.InstallationSlipIssueOffice = "0015"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR110"; param.DocumentNo = "000012"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0012"; param.OtherKey.QuotationTargetCode = "FN0000000123"; param.OtherKey.Alphabet = "12"; param.OtherKey.ProjectCode = "P0000127"; param.OtherKey.BillingTargetCode = "1230000126-001"; param.OtherKey.InstrumentCode = "ICTest12"; param.OtherKey.ContractOffice = "0012"; param.OtherKey.OperationOffice = "0013"; param.OtherKey.BillingOffice = "0014"; param.OtherKey.InstallationSlipIssueOffice = "0016"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR120"; param.DocumentNo = "000013"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0013"; param.OtherKey.QuotationTargetCode = "FN0000000132"; param.OtherKey.Alphabet = "13"; param.OtherKey.ProjectCode = "P0000139"; param.OtherKey.BillingTargetCode = "1230000132-001"; param.OtherKey.InstrumentCode = "ICTest13"; param.OtherKey.ContractOffice = "0013"; param.OtherKey.OperationOffice = "0014"; param.OtherKey.BillingOffice = "0015"; param.OtherKey.InstallationSlipIssueOffice = "0017"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "CTR130"; param.DocumentNo = "000014"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0014"; param.OtherKey.QuotationTargetCode = "FN0000000143"; param.OtherKey.Alphabet = "14"; param.OtherKey.ProjectCode = "P0000148"; param.OtherKey.BillingTargetCode = "1230000149-001"; param.OtherKey.InstrumentCode = "ICTest14"; param.OtherKey.ContractOffice = "0014"; param.OtherKey.OperationOffice = "0015"; param.OtherKey.BillingOffice = "0016"; param.OtherKey.InstallationSlipIssueOffice = "0018"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ICR010"; param.DocumentNo = "000015"; param.DocumentData = ""; param.OtherKey.ContractCode = "N1230000014"; param.OtherKey.ContractOCC = "0015"; param.OtherKey.QuotationTargetCode = "FN0000000151"; param.OtherKey.Alphabet = "15"; param.OtherKey.ProjectCode = "P0000153"; param.OtherKey.BillingTargetCode = "1230000153-001"; param.OtherKey.InstrumentCode = "ICTest15"; param.OtherKey.ContractOffice = "0015"; param.OtherKey.OperationOffice = "0016"; param.OtherKey.BillingOffice = "0017"; param.OtherKey.InstallationSlipIssueOffice = "0019"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ICR020"; param.DocumentNo = "000016"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0016"; param.OtherKey.QuotationTargetCode = "FN0000000160"; param.OtherKey.Alphabet = "16"; param.OtherKey.ProjectCode = "P0000164"; param.OtherKey.BillingTargetCode = "1230000166-001"; param.OtherKey.InstrumentCode = "ICTest16"; param.OtherKey.ContractOffice = "0016"; param.OtherKey.OperationOffice = "0017"; param.OtherKey.BillingOffice = "0018"; param.OtherKey.InstallationSlipIssueOffice = "0020"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ICR030"; param.DocumentNo = "000017"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0017"; param.OtherKey.QuotationTargetCode = "FN0000000172"; param.OtherKey.Alphabet = "17"; param.OtherKey.ProjectCode = "P0000173"; param.OtherKey.BillingTargetCode = "1230000178-001"; param.OtherKey.InstrumentCode = "ICTest17"; param.OtherKey.ContractOffice = "0017"; param.OtherKey.OperationOffice = "0018"; param.OtherKey.BillingOffice = "0019"; param.OtherKey.InstallationSlipIssueOffice = "0021"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR010"; param.DocumentNo = "000018"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0018"; param.OtherKey.QuotationTargetCode = "FN0000000183"; param.OtherKey.Alphabet = "18"; param.OtherKey.ProjectCode = "P0000187"; param.OtherKey.BillingTargetCode = "1230000180-001"; param.OtherKey.InstrumentCode = "ICTest18"; param.OtherKey.ContractOffice = "0018"; param.OtherKey.OperationOffice = "0019"; param.OtherKey.BillingOffice = "0020"; param.OtherKey.InstallationSlipIssueOffice = "0022"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR011"; param.DocumentNo = "000019"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0019"; param.OtherKey.QuotationTargetCode = "FN0000000191"; param.OtherKey.Alphabet = "19"; param.OtherKey.ProjectCode = "P0000192"; param.OtherKey.BillingTargetCode = "1230000194-001"; param.OtherKey.InstrumentCode = "ICTest19"; param.OtherKey.ContractOffice = "0019"; param.OtherKey.OperationOffice = "0020"; param.OtherKey.BillingOffice = "0021"; param.OtherKey.InstallationSlipIssueOffice = "0023"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR012"; param.DocumentNo = "000020"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0020"; param.OtherKey.QuotationTargetCode = "FN0000000206"; param.OtherKey.Alphabet = "20"; param.OtherKey.ProjectCode = "P0000200"; param.OtherKey.BillingTargetCode = "1230000207-001"; param.OtherKey.InstrumentCode = "ICTest20"; param.OtherKey.ContractOffice = "0020"; param.OtherKey.OperationOffice = "0021"; param.OtherKey.BillingOffice = "0022"; param.OtherKey.InstallationSlipIssueOffice = "0024"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR013"; param.DocumentNo = "000021"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0021"; param.OtherKey.QuotationTargetCode = "FN0000000214"; param.OtherKey.Alphabet = "21"; param.OtherKey.ProjectCode = "P0000214"; param.OtherKey.BillingTargetCode = "1230000218-001"; param.OtherKey.InstrumentCode = "ICTest21"; param.OtherKey.ContractOffice = "0021"; param.OtherKey.OperationOffice = "0022"; param.OtherKey.BillingOffice = "0023"; param.OtherKey.InstallationSlipIssueOffice = "0025"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR020"; param.DocumentNo = "000022"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0022"; param.OtherKey.QuotationTargetCode = "FN0000000228"; param.OtherKey.Alphabet = "22"; param.OtherKey.ProjectCode = "P0000226"; param.OtherKey.BillingTargetCode = "1230000220-001"; param.OtherKey.InstrumentCode = "ICTest22"; param.OtherKey.ContractOffice = "0022"; param.OtherKey.OperationOffice = "0023"; param.OtherKey.BillingOffice = "0024"; param.OtherKey.InstallationSlipIssueOffice = "0026"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR021"; param.DocumentNo = "000023"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0023"; param.OtherKey.QuotationTargetCode = "FN0000000239"; param.OtherKey.Alphabet = "23"; param.OtherKey.ProjectCode = "P0000239"; param.OtherKey.BillingTargetCode = "1230000231-001"; param.OtherKey.InstrumentCode = "ICTest23"; param.OtherKey.ContractOffice = "0023"; param.OtherKey.OperationOffice = "0024"; param.OtherKey.BillingOffice = "0025"; param.OtherKey.InstallationSlipIssueOffice = "0027"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR022"; param.DocumentNo = "000024"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0024"; param.OtherKey.QuotationTargetCode = "FN0000000243"; param.OtherKey.Alphabet = "24"; param.OtherKey.ProjectCode = "P0000246"; param.OtherKey.BillingTargetCode = "1230000246-001"; param.OtherKey.InstrumentCode = "ICTest24"; param.OtherKey.ContractOffice = "0024"; param.OtherKey.OperationOffice = "0025"; param.OtherKey.BillingOffice = "0026"; param.OtherKey.InstallationSlipIssueOffice = "0028"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR023"; param.DocumentNo = "000025"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0025"; param.OtherKey.QuotationTargetCode = "FN0000000254"; param.OtherKey.Alphabet = "25"; param.OtherKey.ProjectCode = "P0000256"; param.OtherKey.BillingTargetCode = "1230000258-001"; param.OtherKey.InstrumentCode = "ICTest25"; param.OtherKey.ContractOffice = "0025"; param.OtherKey.OperationOffice = "0026"; param.OtherKey.BillingOffice = "0027"; param.OtherKey.InstallationSlipIssueOffice = "0029"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR030"; param.DocumentNo = "000026"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0026"; param.OtherKey.QuotationTargetCode = "FN0000000265"; param.OtherKey.Alphabet = "26"; param.OtherKey.ProjectCode = "P0000265"; param.OtherKey.BillingTargetCode = "1230000264-001"; param.OtherKey.InstrumentCode = "ICTest26"; param.OtherKey.ContractOffice = "0026"; param.OtherKey.OperationOffice = "0027"; param.OtherKey.BillingOffice = "0028"; param.OtherKey.InstallationSlipIssueOffice = "0030"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR031"; param.DocumentNo = "000027"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0027"; param.OtherKey.QuotationTargetCode = "FN0000000272"; param.OtherKey.Alphabet = "27"; param.OtherKey.ProjectCode = "P0000274"; param.OtherKey.BillingTargetCode = "1230000279-001"; param.OtherKey.InstrumentCode = "ICTest27"; param.OtherKey.ContractOffice = "0027"; param.OtherKey.OperationOffice = "0028"; param.OtherKey.BillingOffice = "0029"; param.OtherKey.InstallationSlipIssueOffice = "0031"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR032"; param.DocumentNo = "000028"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0028"; param.OtherKey.QuotationTargetCode = "FN0000000281"; param.OtherKey.Alphabet = "28"; param.OtherKey.ProjectCode = "P0000282"; param.OtherKey.BillingTargetCode = "1230000287-001"; param.OtherKey.InstrumentCode = "ICTest28"; param.OtherKey.ContractOffice = "0028"; param.OtherKey.OperationOffice = "0029"; param.OtherKey.BillingOffice = "0030"; param.OtherKey.InstallationSlipIssueOffice = "0032"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR033"; param.DocumentNo = "000029"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0029"; param.OtherKey.QuotationTargetCode = "FN0000000297"; param.OtherKey.Alphabet = "29"; param.OtherKey.ProjectCode = "P0000294"; param.OtherKey.BillingTargetCode = "1230000292-001"; param.OtherKey.InstrumentCode = "ICTest29"; param.OtherKey.ContractOffice = "0029"; param.OtherKey.OperationOffice = "0030"; param.OtherKey.BillingOffice = "0031"; param.OtherKey.InstallationSlipIssueOffice = "0033"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR040"; param.DocumentNo = "000030"; param.DocumentData = ""; param.OtherKey.ContractCode = "Q1230000014"; param.OtherKey.ContractOCC = "0030"; param.OtherKey.QuotationTargetCode = "FN0000000304"; param.OtherKey.Alphabet = "30"; param.OtherKey.ProjectCode = "P0000300"; param.OtherKey.BillingTargetCode = "1230000303-001"; param.OtherKey.InstrumentCode = "ICTest30"; param.OtherKey.ContractOffice = "0030"; param.OtherKey.OperationOffice = "0031"; param.OtherKey.BillingOffice = "0032"; param.OtherKey.InstallationSlipIssueOffice = "0034"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR041"; param.DocumentNo = "000031"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0031"; param.OtherKey.QuotationTargetCode = "FN0000000316"; param.OtherKey.Alphabet = "31"; param.OtherKey.ProjectCode = "P0000313"; param.OtherKey.BillingTargetCode = "1230000314-001"; param.OtherKey.InstrumentCode = "ICTest31"; param.OtherKey.ContractOffice = "0031"; param.OtherKey.OperationOffice = "0032"; param.OtherKey.BillingOffice = "0033"; param.OtherKey.InstallationSlipIssueOffice = "0035"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR042"; param.DocumentNo = "000032"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0032"; param.OtherKey.QuotationTargetCode = "FN0000000328"; param.OtherKey.Alphabet = "32"; param.OtherKey.ProjectCode = "P0000328"; param.OtherKey.BillingTargetCode = "1230000321-001"; param.OtherKey.InstrumentCode = "ICTest32"; param.OtherKey.ContractOffice = "0032"; param.OtherKey.OperationOffice = "0033"; param.OtherKey.BillingOffice = "0034"; param.OtherKey.InstallationSlipIssueOffice = "0036"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR043"; param.DocumentNo = "000033"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0033"; param.OtherKey.QuotationTargetCode = "FN0000000339"; param.OtherKey.Alphabet = "33"; param.OtherKey.ProjectCode = "P0000339"; param.OtherKey.BillingTargetCode = "1230000330-001"; param.OtherKey.InstrumentCode = "ICTest33"; param.OtherKey.ContractOffice = "0033"; param.OtherKey.OperationOffice = "0034"; param.OtherKey.BillingOffice = "0035"; param.OtherKey.InstallationSlipIssueOffice = "0037"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR044"; param.DocumentNo = "000034"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0034"; param.OtherKey.QuotationTargetCode = "FN0000000342"; param.OtherKey.Alphabet = "34"; param.OtherKey.ProjectCode = "P0000344"; param.OtherKey.BillingTargetCode = "1230000345-001"; param.OtherKey.InstrumentCode = "ICTest34"; param.OtherKey.ContractOffice = "0034"; param.OtherKey.OperationOffice = "0035"; param.OtherKey.BillingOffice = "0036"; param.OtherKey.InstallationSlipIssueOffice = "0038"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR045"; param.DocumentNo = "000035"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0035"; param.OtherKey.QuotationTargetCode = "FN0000000354"; param.OtherKey.Alphabet = "35"; param.OtherKey.ProjectCode = "P0000351"; param.OtherKey.BillingTargetCode = "1230000359-001"; param.OtherKey.InstrumentCode = "ICTest35"; param.OtherKey.ContractOffice = "0035"; param.OtherKey.OperationOffice = "0036"; param.OtherKey.BillingOffice = "0037"; param.OtherKey.InstallationSlipIssueOffice = "0039"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR050"; param.DocumentNo = "000036"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0036"; param.OtherKey.QuotationTargetCode = "FN0000000360"; param.OtherKey.Alphabet = "36"; param.OtherKey.ProjectCode = "P0000362"; param.OtherKey.BillingTargetCode = "1230000367-001"; param.OtherKey.InstrumentCode = "ICTest36"; param.OtherKey.ContractOffice = "0036"; param.OtherKey.OperationOffice = "0037"; param.OtherKey.BillingOffice = "0038"; param.OtherKey.InstallationSlipIssueOffice = "0040"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR051"; param.DocumentNo = "000037"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0037"; param.OtherKey.QuotationTargetCode = "FN0000000374"; param.OtherKey.Alphabet = "37"; param.OtherKey.ProjectCode = "P0000374"; param.OtherKey.BillingTargetCode = "1230000377-001"; param.OtherKey.InstrumentCode = "ICTest37"; param.OtherKey.ContractOffice = "0037"; param.OtherKey.OperationOffice = "0038"; param.OtherKey.BillingOffice = "0039"; param.OtherKey.InstallationSlipIssueOffice = "0041"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR060"; param.DocumentNo = "000038"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0038"; param.OtherKey.QuotationTargetCode = "FN0000000385"; param.OtherKey.Alphabet = "38"; param.OtherKey.ProjectCode = "P0000385"; param.OtherKey.BillingTargetCode = "1230000388-001"; param.OtherKey.InstrumentCode = "ICTest38"; param.OtherKey.ContractOffice = "0038"; param.OtherKey.OperationOffice = "0039"; param.OtherKey.BillingOffice = "0040"; param.OtherKey.InstallationSlipIssueOffice = "0042"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR070"; param.DocumentNo = "000039"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0039"; param.OtherKey.QuotationTargetCode = "FN0000000393"; param.OtherKey.Alphabet = "39"; param.OtherKey.ProjectCode = "P0000396"; param.OtherKey.BillingTargetCode = "1230000398-001"; param.OtherKey.InstrumentCode = "ICTest39"; param.OtherKey.ContractOffice = "0039"; param.OtherKey.OperationOffice = "0040"; param.OtherKey.BillingOffice = "0041"; param.OtherKey.InstallationSlipIssueOffice = "0043"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR080"; param.DocumentNo = "000040"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0040"; param.OtherKey.QuotationTargetCode = "FN0000000405"; param.OtherKey.Alphabet = "40"; param.OtherKey.ProjectCode = "P0000407"; param.OtherKey.BillingTargetCode = "1230000400-001"; param.OtherKey.InstrumentCode = "ICTest40"; param.OtherKey.ContractOffice = "0040"; param.OtherKey.OperationOffice = "0041"; param.OtherKey.BillingOffice = "0042"; param.OtherKey.InstallationSlipIssueOffice = "0044"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "ISR090"; param.DocumentNo = "000041"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0041"; param.OtherKey.QuotationTargetCode = "FN0000000416"; param.OtherKey.Alphabet = "41"; param.OtherKey.ProjectCode = "P0000410"; param.OtherKey.BillingTargetCode = "1230000415-001"; param.OtherKey.InstrumentCode = "ICTest41"; param.OtherKey.ContractOffice = "0041"; param.OtherKey.OperationOffice = "0042"; param.OtherKey.BillingOffice = "0043"; param.OtherKey.InstallationSlipIssueOffice = "0045"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR010"; param.DocumentNo = "000042"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0042"; param.OtherKey.QuotationTargetCode = "FN0000000422"; param.OtherKey.Alphabet = "42"; param.OtherKey.ProjectCode = "P0000428"; param.OtherKey.BillingTargetCode = "1230000426-001"; param.OtherKey.InstrumentCode = "ICTest42"; param.OtherKey.ContractOffice = "0042"; param.OtherKey.OperationOffice = "0043"; param.OtherKey.BillingOffice = "0044"; param.OtherKey.InstallationSlipIssueOffice = "0046"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR020"; param.DocumentNo = "000043"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0043"; param.OtherKey.QuotationTargetCode = "FN0000000431"; param.OtherKey.Alphabet = "43"; param.OtherKey.ProjectCode = "P0000439"; param.OtherKey.BillingTargetCode = "1230000434-001"; param.OtherKey.InstrumentCode = "ICTest43"; param.OtherKey.ContractOffice = "0043"; param.OtherKey.OperationOffice = "0044"; param.OtherKey.BillingOffice = "0045"; param.OtherKey.InstallationSlipIssueOffice = "0047"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR030"; param.DocumentNo = "000044"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0044"; param.OtherKey.QuotationTargetCode = "FN0000000447"; param.OtherKey.Alphabet = "44"; param.OtherKey.ProjectCode = "P0000446"; param.OtherKey.BillingTargetCode = "1230000442-001"; param.OtherKey.InstrumentCode = "ICTest44"; param.OtherKey.ContractOffice = "0044"; param.OtherKey.OperationOffice = "0045"; param.OtherKey.BillingOffice = "0046"; param.OtherKey.InstallationSlipIssueOffice = "0048"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR040"; param.DocumentNo = "000045"; param.DocumentData = ""; param.OtherKey.ContractCode = "MA1230000014"; param.OtherKey.ContractOCC = "0045"; param.OtherKey.QuotationTargetCode = "FN0000000459"; param.OtherKey.Alphabet = "45"; param.OtherKey.ProjectCode = "P0000453"; param.OtherKey.BillingTargetCode = "1230000451-001"; param.OtherKey.InstrumentCode = "ICTest45"; param.OtherKey.ContractOffice = "0045"; param.OtherKey.OperationOffice = "0046"; param.OtherKey.BillingOffice = "0047"; param.OtherKey.InstallationSlipIssueOffice = "0049"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR050"; param.DocumentNo = "000046"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0046"; param.OtherKey.QuotationTargetCode = "FN0000000464"; param.OtherKey.Alphabet = "46"; param.OtherKey.ProjectCode = "P0000465"; param.OtherKey.BillingTargetCode = "1230000468-001"; param.OtherKey.InstrumentCode = "ICTest46"; param.OtherKey.ContractOffice = "0046"; param.OtherKey.OperationOffice = "0047"; param.OtherKey.BillingOffice = "0048"; param.OtherKey.InstallationSlipIssueOffice = "0050"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR060"; param.DocumentNo = "000047"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0047"; param.OtherKey.QuotationTargetCode = "FN0000000474"; param.OtherKey.Alphabet = "47"; param.OtherKey.ProjectCode = "P0000474"; param.OtherKey.BillingTargetCode = "1230000473-001"; param.OtherKey.InstrumentCode = "ICTest47"; param.OtherKey.ContractOffice = "0047"; param.OtherKey.OperationOffice = "0048"; param.OtherKey.BillingOffice = "0049"; param.OtherKey.InstallationSlipIssueOffice = "0051"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR070"; param.DocumentNo = "000048"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0048"; param.OtherKey.QuotationTargetCode = "FN0000000486"; param.OtherKey.Alphabet = "48"; param.OtherKey.ProjectCode = "P0000481"; param.OtherKey.BillingTargetCode = "1230000487-001"; param.OtherKey.InstrumentCode = "ICTest48"; param.OtherKey.ContractOffice = "0048"; param.OtherKey.OperationOffice = "0049"; param.OtherKey.BillingOffice = "0050"; param.OtherKey.InstallationSlipIssueOffice = "0052"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR080"; param.DocumentNo = "000049"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0049"; param.OtherKey.QuotationTargetCode = "FN0000000492"; param.OtherKey.Alphabet = "49"; param.OtherKey.ProjectCode = "P0000490"; param.OtherKey.BillingTargetCode = "1230000494-001"; param.OtherKey.InstrumentCode = "ICTest49"; param.OtherKey.ContractOffice = "0049"; param.OtherKey.OperationOffice = "0050"; param.OtherKey.BillingOffice = "0051"; param.OtherKey.InstallationSlipIssueOffice = "0053"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR090"; param.DocumentNo = "000050"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0050"; param.OtherKey.QuotationTargetCode = "FN0000000508"; param.OtherKey.Alphabet = "50"; param.OtherKey.ProjectCode = "P0000502"; param.OtherKey.BillingTargetCode = "1230000505-001"; param.OtherKey.InstrumentCode = "ICTest50"; param.OtherKey.ContractOffice = "0050"; param.OtherKey.OperationOffice = "0051"; param.OtherKey.BillingOffice = "0052"; param.OtherKey.InstallationSlipIssueOffice = "0054"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR110"; param.DocumentNo = "000051"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0051"; param.OtherKey.QuotationTargetCode = "FN0000000512"; param.OtherKey.Alphabet = "51"; param.OtherKey.ProjectCode = "P0000516"; param.OtherKey.BillingTargetCode = "1230000519-001"; param.OtherKey.InstrumentCode = "ICTest51"; param.OtherKey.ContractOffice = "0051"; param.OtherKey.OperationOffice = "0052"; param.OtherKey.BillingOffice = "0053"; param.OtherKey.InstallationSlipIssueOffice = "0055"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR120"; param.DocumentNo = "000052"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0052"; param.OtherKey.QuotationTargetCode = "FN0000000523"; param.OtherKey.Alphabet = "52"; param.OtherKey.ProjectCode = "P0000525"; param.OtherKey.BillingTargetCode = "1230000520-001"; param.OtherKey.InstrumentCode = "ICTest52"; param.OtherKey.ContractOffice = "0052"; param.OtherKey.OperationOffice = "0053"; param.OtherKey.BillingOffice = "0054"; param.OtherKey.InstallationSlipIssueOffice = "0056"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR130"; param.DocumentNo = "000053"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0053"; param.OtherKey.QuotationTargetCode = "FN0000000539"; param.OtherKey.Alphabet = "53"; param.OtherKey.ProjectCode = "P0000535"; param.OtherKey.BillingTargetCode = "1230000535-001"; param.OtherKey.InstrumentCode = "ICTest53"; param.OtherKey.ContractOffice = "0053"; param.OtherKey.OperationOffice = "0054"; param.OtherKey.BillingOffice = "0055"; param.OtherKey.InstallationSlipIssueOffice = "0057"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR140"; param.DocumentNo = "000054"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0054"; param.OtherKey.QuotationTargetCode = "FN0000000544"; param.OtherKey.Alphabet = "54"; param.OtherKey.ProjectCode = "P0000542"; param.OtherKey.BillingTargetCode = "1230000546-001"; param.OtherKey.InstrumentCode = "ICTest54"; param.OtherKey.ContractOffice = "0054"; param.OtherKey.OperationOffice = "0055"; param.OtherKey.BillingOffice = "0056"; param.OtherKey.InstallationSlipIssueOffice = "0058"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR150"; param.DocumentNo = "000055"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0055"; param.OtherKey.QuotationTargetCode = "FN0000000554"; param.OtherKey.Alphabet = "55"; param.OtherKey.ProjectCode = "P0000553"; param.OtherKey.BillingTargetCode = "1230000553-001"; param.OtherKey.InstrumentCode = "ICTest55"; param.OtherKey.ContractOffice = "0055"; param.OtherKey.OperationOffice = "0056"; param.OtherKey.BillingOffice = "0057"; param.OtherKey.InstallationSlipIssueOffice = "0059"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR170"; param.DocumentNo = "000056"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0056"; param.OtherKey.QuotationTargetCode = "FN0000000562"; param.OtherKey.Alphabet = "56"; param.OtherKey.ProjectCode = "P0000569"; param.OtherKey.BillingTargetCode = "1230000567-001"; param.OtherKey.InstrumentCode = "ICTest56"; param.OtherKey.ContractOffice = "0056"; param.OtherKey.OperationOffice = "0057"; param.OtherKey.BillingOffice = "0058"; param.OtherKey.InstallationSlipIssueOffice = "0060"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR180"; param.DocumentNo = "000057"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0057"; param.OtherKey.QuotationTargetCode = "FN0000000578"; param.OtherKey.Alphabet = "57"; param.OtherKey.ProjectCode = "P0000578"; param.OtherKey.BillingTargetCode = "1230000574-001"; param.OtherKey.InstrumentCode = "ICTest57"; param.OtherKey.ContractOffice = "0057"; param.OtherKey.OperationOffice = "0058"; param.OtherKey.BillingOffice = "0059"; param.OtherKey.InstallationSlipIssueOffice = "0061"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR190"; param.DocumentNo = "000058"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0058"; param.OtherKey.QuotationTargetCode = "FN0000000583"; param.OtherKey.Alphabet = "58"; param.OtherKey.ProjectCode = "P0000587"; param.OtherKey.BillingTargetCode = "1230000580-001"; param.OtherKey.InstrumentCode = "ICTest58"; param.OtherKey.ContractOffice = "0058"; param.OtherKey.OperationOffice = "0059"; param.OtherKey.BillingOffice = "0060"; param.OtherKey.InstallationSlipIssueOffice = "0062"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);
                param.DocumentCode = "IVR191"; param.DocumentNo = "000059"; param.DocumentData = ""; param.OtherKey.ContractCode = "SG1230000014"; param.OtherKey.ContractOCC = "0059"; param.OtherKey.QuotationTargetCode = "FN0000000599"; param.OtherKey.Alphabet = "59"; param.OtherKey.ProjectCode = "P0000594"; param.OtherKey.BillingTargetCode = "1230000598-001"; param.OtherKey.InstrumentCode = "ICTest59"; param.OtherKey.ContractOffice = "0059"; param.OtherKey.OperationOffice = "0060"; param.OtherKey.BillingOffice = "0061"; param.OtherKey.InstallationSlipIssueOffice = "0063"; param.OtherKey.MonthYear = start.AddDays(1);
                target.GenerateDocument(param);

            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            // TODO Akat K. : compare result
            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, actual));
        }
    }
}