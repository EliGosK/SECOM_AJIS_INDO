using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SECOM_AJIS.DataEntity.Installation
{
    public interface IInstallationDocumentHandler
    {
        /// <summary>
        /// Create report installation (new installation slip rental,change installation slip,removal installation slip,new installation slip sale)
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        Stream CreateInstallationReport(string strSlipNo, string strDocumentCode);
        /// <summary>
        /// Create report installation complete confirm
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        Stream CreateReportInstallCompleteConfirmData(string strSlipNo);
        /// <summary>
        /// Create report delivery confirm
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        Stream CreateReportDeliveryConfirmData(string strSlipNo);
        /// <summary>
        /// Create report installation complete confirm
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        Stream CreateReportInstallationCompleteConfirmation(string strSlipNo);
        /// <summary>
        /// ISS060 Create Roport ISR110 and ISR111
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        Stream ISS060CreateReportISR11_ISR111(string strSlipNo);
        /// <summary>
        /// Create report PO and Sub price
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        Stream CreateReportInstallationPOandSubPrice(string strMaintenanceNo, string strSubcontractorCode, string nameSignature);
        /// <summary>
        /// Create report installation request
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        Stream CreateReportInstallationRequestData(string strMaintenanceNo);
        /// <summary>
        /// Create report installation spec complete
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        Stream CreateReportInstallSpecCompleteData(string strMaintenanceNo, string strSubcontractorCode);
        /// <summary>
        /// Create report IE check sheet
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        Stream CreateReportIECheckSheetData(string strMaintenanceNo, string strSubcontractorCode);
        /// <summary>
        /// Create report acceptance inspection notice
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        Stream CreateReportAcceptanceInspectionNotice(string strMaintenanceNo, string strSubcontractorCode);
        /// <summary>
        /// Create report and merged all for screen ISS050
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        Stream CreateReportISS050MergedAll(string strMaintenanceNo, string strSubcontractorCode, string nameSignature);
        /// <summary>
        /// Create report ISR010 and return file path
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        string CreateReportInstallationRequestFilePath(string strMaintenanceNo);

        /// <summary>
        /// Create report ISR120 and return file path
        /// </summary>
        /// <param name="paramSearch"></param>
        /// <returns></returns>
        string GenerateISR120Report(List<dtGetInstallationReport> data,doInstallationReport paramSearch);

        /// <summary>
        /// Create report ISR130 and return file path
        /// </summary>
        /// <param name="dtGetInstallationReportMonthly"></param>
        /// <returns></returns>
        string GenerateISR130Report(List<dtGetInstallationReportMonthly> data, doInstallationReportMonthly paramSearch);

        /// <summary>
        /// Create report ISR140 and return file path
        /// </summary>
        /// <param name="dtGetInstallationReportMonthly"></param>
        /// <returns></returns>
        string GenerateISR140Report(List<dtGetInstallationReportMonthly> data, doInstallationReportMonthly paramSearch);

        List<tbt_InstallationReprint> GetTbt_InstallationReprint();
    }
    
}
