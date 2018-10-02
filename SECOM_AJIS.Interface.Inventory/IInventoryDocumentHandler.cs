using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Quotation;
using System.IO;


namespace SECOM_AJIS.DataEntity.Inventory
{
    public interface IInventoryDocumentHandler
    {

        // ------ Get data for generate report ------
        List<doIVR> GetIVR(string strInventorySlipNo);
        List<doIVR100> GetIVR100(string strInventorySlipNo);
        List<doIVR110> GetIVR110(string strInventorySlipNo);


        // ------ Geneate report ------
        /// <summary>
        /// Generate report IVR010 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR010(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR020 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR020(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR030 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR030(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR040 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR040(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR050 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR050(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR060 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR060(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR070 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR070(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR080 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR080(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR090 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR090(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR120 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR120(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR130 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR130(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR140 and return as Stream.
        /// </summary>
        /// <param name="dtReportDateTime"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR140(DateTime dtReportDateTime, string strEmpNo, DateTime dtDateTime); // IVR140 IVR141 IVR142 IVR143

        /// <summary>
        /// Generate report IVR150 and return as Stream.
        /// </summary>
        /// <param name="dtReportDateTime"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR150(string monthYearGenerate, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR170 and return as Stream.
        /// </summary>
        /// <param name="strPickingListNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR170(string strPickingListNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR180 and return as Stream.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR180(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR190 and return as Stream.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <param name="strOfficeCode"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR190(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR191 and return as Stream.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <param name="strOfficeCode"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR191(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR192 and return as Stream.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <param name="strOfficeCode"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateIVR192(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate returning report.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        Stream GenerateIVR210(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR010 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR010FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR020 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR020FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR030 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR030FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR040 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR040FilePath(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR050 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR050FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR060 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR060FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR070 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR070FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR080 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR080FilePath(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR090 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR090FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR100 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR100FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR110 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR110FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR120 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR120FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR130 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR130FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR140 and return file path.
        /// </summary>
        /// <param name="dtReportDateTime"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR140FilePath(DateTime dtReportDateTime, string strEmpNo, DateTime dtDateTime); // IVR140 IVR141 IVR142 IVR143

        /// <summary>
        /// Generate report IVR150 and return file path.
        /// </summary>
        /// <param name="dtReportDateTime"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR150FilePath(string monthYearGenerate, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR170 and return file path.
        /// </summary>
        /// <param name="strPickingListNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR170FilePath(string strPickingListNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR180 and return file path.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strInventorySlipIssueOffice"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR180FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR190 and return file path.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <param name="strOfficeCode"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR190FilePath(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report IVR191 and return file path.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <param name="strOfficeCode"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR191FilePath(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate returning report.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        string GenerateIVR210FilePath(string strInventorySlipNoList, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Genereate Inventory Account Data as CSV report.
        /// </summary>
        /// <param name="dtDateGenerate"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtProcessTime"></param>
        bool GenerateInventoryAccountData(DateTime dtDateGenerate, string strEmpNo, DateTime dtProcessTime);

        /// <summary>
        /// Generate Stock Report - In Report
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS280InReport(string reportType, List<dtInReportDetail> data);

        /// <summary>
        /// Generate Stock Report - In Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS280InReportSummary(string reportType, List<dtInReportDetail> data);

        /// <summary>
        /// Generate Stock Report - Out Report
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS281OutReport(string reportType, List<dtOutReportDetail> data);

        /// <summary>
        /// Generate Stock Report - Out Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS281OutReportSummary(string reportType, List<dtOutReportDetail> data);

        /// <summary>
        /// Generate Stock Report - Return Report
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS282ReturnReport(string reportType, List<dtReturnReportDetail> data);

        /// <summary>
        /// Generate Stock Report - Return Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS282ReturnReportSummary(string reportType, List<dtReturnReportDetail> data);

        /// <summary>
        /// Generate Stock Report - Movement Report
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS283MovementReport(string reportType, List<dtMovementReport> data);

        /// <summary>
        /// Generate Stock Report - Inprocess-to-Install Report
        /// </summary>
        /// <param name="data"></param>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        string GenerateIVS284InprocessToInstallReport(string reportType, List<dtInprocessToInstallReportDetail> data, doIVS284SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - Inprocess-to-Install Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        string GenerateIVS284InprocessToInstallReportSummary(string reportType, List<dtInprocessToInstallReport> data, doIVS284SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - Physical Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS285PhysicalReport(string reportType, List<dtPhysicalReport> data);

        /// <summary>
        /// Generate Stock Report - InProcess Report
        /// </summary>
        /// <param name="data"></param>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        string GenerateIVS286InProcessReport(string reportType, List<dtInProcessReportDetail> data, doIVS286SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - InProcess Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        string GenerateIVS286InProcessReportSummary(string reportType, List<dtInProcessReport> data, doIVS286SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - Stock List Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS287StockListReport(string reportType, List<dtStockListReport> data);

        /// <summary>
        /// Generate Stock Report - Change Area Report
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS288ChangeAreaReport(List<dtChangeAreaReportDetail> data, doIVS288SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - Change Area Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS288ChangeAreaReportSummary(List<dtChangeAreaReportDetail> data, doIVS288SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - Elimination Report
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS289EliminateReport(List<dtEliminateReportDetail> data, doIVS289SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - Elimination Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS289EliminateReportSummary(List<dtEliminateReportDetail> data, doIVS289SearchCondition searchParam);

        /// <summary>
        /// Generate Stock Report - Buffer Loss Report
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS290BufferLossReport(List<dtBufferLossReportDetail> data);

        /// <summary>
        /// Generate Stock Report - Buffer Loss Report (Summary)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GenerateIVS290BufferLossReportSummary(List<dtBufferLossReportDetail> data);

        /// <summary>
        /// Generate report IVR192 and return file path.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <param name="strOfficeCode"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateIVR192FilePath(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate stock taking result report.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        string GenerateIVS170StockTakingResult(List<dtStockCheckingList> data, doGetStockCheckingList cond);

    }
}
