using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SECOM_AJIS.DataEntity.Installation
{
    public interface IReportHandler
    {
        /// <summary>
        /// Get data for report rental slip
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTNewRentalSlipDo> GetRptNewRentalSlipReport(string strSlipNo);
        /// <summary>
        /// Get data for report rental slip test
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTNewRentalSlipDo> GetRptNewRentalSlipReport_TestData(string strSlipNo);
        /// <summary>
        /// Get data for report change slip
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTChangeSlipDo> GetRptChangeSlipReport(string strSlipNo);
        /// <summary>
        /// Get data for report change slip for test
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTChangeSlipDo> GetRptChangeSlipReport_TestData(string strSlipNo);
        /// <summary>
        /// Get data for report removal slip
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTRemoveSlipDo> GetRptRemoveSlipReport(string strSlipNo);
        /// <summary>
        /// Get data for report remove slip for test
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTRemoveSlipDo> GetRptRemoveSlipReport_TestData(string strSlipNo);
        /// <summary>
        /// Get data for report PO and sub price
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        List<RPTPOSubPriceDo> GetRptPOSubPriceData(string strMaintenanceNo, string strSubcontractorCode);
        /// <summary>
        /// Get data for report sale slip
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTNewSaleSlipDo> GetRptNewSaleSlipReport(string strSlipNo);
        /// <summary>
        /// Get data for report new sale slip for test
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTNewSaleSlipDo> GetRptNewSaleSlipReport_TestData(string strSlipNo);
        /// <summary>
        /// Get data for report installation request
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        List<RPTInstallRequestDo> GetRptInstallationRequestData(string strMaintenanceNo);
        /// <summary>
        /// Get data for report installation spec complete
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        List<RPTInstallSpecCompleteDo> GetRptInstallSpecCompleteData(string strMaintenanceNo, string strSubcontractorCode);
        /// <summary>
        /// Get data for report IE check sheet
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        List<RPTIECheckSheetDo> GetRptIECheckSheetData(string strMaintenanceNo, string strSubcontractorCode);
        /// <summary>
        /// Get data for report installation complete confirm
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTInstallCompleteDo> GetRptInstallCompleteConfirmData(string strSlipNo);
        /// <summary>
        /// Get data for report accept inspec notice
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        List<RPTAcceptInspecDo> GetRptAcceptInspecNocticeData(string strMaintenanceNo, string strSubcontractorCode);
        /// <summary>
        /// Get data for report delivery confirm
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<RPTDeliveryConfirmDo> GetRptDeliveryConfirmData(string strSlipNo);
        /// <summary>
        /// Get Signature data for report
        /// </summary>
        /// <returns></returns>
        List<RptSignatureDo> GetRptSignatureData(string strCode, string strPosition);
        /// <summary>
        /// Get RptISR110 Install Complete Confirm Data
        /// </summary>
        /// <returns></returns>
        List<RptISR110InstallCompleteConfirmDo> GetRptISR110InstallCompleteConfirmData(string strSlipNo);
    }
}
