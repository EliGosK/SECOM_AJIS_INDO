using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Quotation;
using System.IO;
using SECOM_AJIS.Common.Util;


namespace SECOM_AJIS.DataEntity.Inventory
{
    public interface IInventoryHandler
    {
        /// <summary>
        /// To generate inventory stlip no.
        /// </summary>
        /// <param name="officeCode"></param>
        /// <param name="SlipId"></param>
        /// <returns></returns>
        string GenerateInventorySlipNo(string officeCode, string SlipId);

        /// <summary>
        /// Get Head office of inventory.
        /// </summary>
        /// <returns></returns>
        List<doOffice> GetInventoryHeadOffice();

        /// <summary>
        /// Get Srinakarin office of inventory.
        /// </summary>
        /// <returns></returns>
        List<doOffice> GetInventorySrinakarinOffice();

        /// <summary>
        /// Get purchase order for maintian.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doPurchaseOrder> GetPurchaserOrderForMaintain(doPurchaseOrderSearchCond cond);

        /// <summary>
        /// Getting purchase order detail for register stock-in.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <returns></returns>
        List<doPurchaseOrderDetail> GetPurchaseOrderDetailForRegisterStockIn(string strPurchaseOrderNo);

        List<tbs_InventoryRunningSlipNo> GetTbs_InventorySlipRunningNo(string month, string year, string officeCode, string slipid);
        List<tbs_InventoryRunningSlipNo> UpdateTbs_InventorySlipRunningNo(string month, string year, string officeCode, string slipid, string runningNo);
        List<tbs_InventoryRunningSlipNo> InsertTbs_InventorySlipRunningNo(string runningNo, string month, string year, string officeCode, string slipid);
        List<tbt_InventorySlip> InsertTbt_InventorySlip(string xmlTbt_InventorySlilp);
        List<tbt_InventorySlipDetail> InsertTbt_InventorySlipDetail(string xmlTbt_InventorySlipDetail);
        List<tbt_PurchaseOrderDetail> UpdateTbt_PurchaseOrderDetail(string xmlPurchasOrderDetail);
        List<tbt_PurchaseOrder> UpdateTbt_PurchaseOrder(string xmlPurchaseOrder);
        List<tbt_PurchaseOrder> GetTbt_PurchaseOrder(string purchaseOrderNo);
        List<tbt_PurchaseOrderDetail> GetTbt_PurchaseOrderDetail(string purchaseOrderNo, string instrumentCode);
        List<tbt_InventoryCurrent> GetTbt_InventoryCurrent(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode);
        List<tbt_InventoryCurrent> UpdateTbt_InventoryCurrent(string xmlTbt_InventoryCurrent);

        /// <summary>
        /// Getting inventory slip detail for search.
        /// </summary>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<doInventorySlipDetailList> GetInventorySlipDetailForSearch(string slipNo);

        /// <summary>
        /// Getting inventory slip for search.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        List<doInventorySlipList> GetInventorySlipForSearch(doInventorySlipSearchCondition Cond);

        List<tbt_InventorySlip> GetTbt_InventorySlip(string slipNo);
        List<tbt_InventorySlip> GetTbt_InventorySlip(string slipNo, string installationSlipNo);
        List<tbt_InventorySlip> UpdateTbt_InventorySlip(List<tbt_InventorySlip> lstSlip);
        List<tbt_InventorySlipDetail> DeleteTbt_InventorySlipDetail(string slipNo);
        List<tbt_InventorySlip> DeleteTbt_InventorySlip(string slipNo);

        /// <summary>
        /// Get inventory slip detail for register.
        /// </summary>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<doInventorySlipDetailList> GetInventorySlipDetailForRegister(string slipNo);

        /// <summary>
        /// Update inventory current destination.
        /// </summary>
        /// <param name="doInventoryCurrent"></param>
        /// <returns></returns>
        bool UpdateInventoryCurrentDestination(List<tbt_InventoryCurrent> doInventoryCurrent);

        List<tbt_InventoryCurrent> InsertTbt_InventoryCurrent(string xmlTbt_InventoryCurrent);
        List<tbt_AccountInstock> UpdateTbt_AccountInStock(List<tbt_AccountInstock> newAccountInStock);
        List<tbt_AccountSampleInstock> UpdateTbt_AccountSampleInStock(List<tbt_AccountSampleInstock> lstInStock);
        List<tbt_AccountSampleInprocess> UpdateTbt_AccountSampleInProcess(string xmlTbt_AccountSampleInProcess);
        List<tbt_AccountSampleInprocess> GetTbt_AccountSampleInProcess(string locationCode, string contractCode, string instrumentCode);
              
        List<tbt_InventoryCurrent> UpdateTbt_InventoryCurrent(List<tbt_InventoryCurrent> lstCurrent);
        List<tbt_InventoryCurrent> InsertTbt_InventoryCurrent(List<tbt_InventoryCurrent> lstCurrent);

        List<tbt_InventorySlip> InsertTbt_InventorySlip(List<tbt_InventorySlip> lstSlip);
        List<tbt_InventorySlipDetail> InsertTbt_InventorySlipDetail(List<tbt_InventorySlipDetail> lstSlipDetail);

        /// <summary>
        /// For update transfer account data of sample instrument.
        /// </summary>
        /// <param name="doGroupSample"></param>
        /// <param name="decMovingAveragePrice"></param>
        /// <returns></returns>
        bool UpdateAccountTransferSampleInstrument(doGroupSampleInstrument doGroupSample, decimal? decMovingAveragePrice);
        bool UpdateAccountTransferSampleInstrument(doGroupSampleInstrument doGroupSample, decimal? decMovingAveragePrice, out int? intReturnInprocess); // New WIP concept @ 24-Feb-2015

        /// <summary>
        /// For calculate moveing average price.
        /// </summary>
        /// <param name="doGroupNew"></param>
        /// <returns></returns>
        [Obsolete("use GetMonthlyAveragePrice() instead")]
        decimal CalculateMovingAveragePrice(doGroupNewInstrument doGroupNew);

        /// <summary>
        /// Update transfer new instrument to Account DB.
        /// </summary>
        /// <param name="doGroupNew"></param>
        /// <param name="decMovingAveragePrice"></param>
        /// <returns></returns>
        bool UpdateAccountTransferNewInstrument(doGroupNewInstrument doGroupNew, decimal? decMovingAveragePrice);
        bool UpdateAccountTransferNewInstrument(doGroupNewInstrument doGroupNew, decimal? decMovingAveragePrice, out int? intReturnInprocess); // New WIP concept @ 24-Feb-2015

        List<tbs_PurchaseOrderRunningNo> GetTbs_PurchaseOrderRunningNo(string yearCode, string monthCode, string nationCodeCode);

        /// <summary>
        /// Check transfer qty with current qty on DB.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        doCheckTransferQtyResult CheckTransferQty(doCheckTransferQty Cond);

        /// <summary>
        /// Check transfer qty with current qty on DB (Absolute QTY).
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        doCheckTransferQtyResult CheckTransferQtyIVS190(doCheckTransferQty Cond);

        /// <summary>
        /// Generate purchase order number.
        /// </summary>
        /// <param name="strRegionCode"></param>
        /// <returns></returns>
        string GeneratePurchaseOrderNo(string strRegionCode);

        List<tbt_PurchaseOrder> InsertTbt_PurchaseOrder(List<tbt_PurchaseOrder> lst);
        List<tbt_PurchaseOrderDetail> InsertTbt_PurchaseOrderDetail(List<tbt_PurchaseOrderDetail> lst);

        /// <summary>
        /// Search inventory instrument list.
        /// </summary>
        /// <param name="officeCode"></param>
        /// <param name="locationCode"></param>
        /// <param name="areaCodeStr"></param>
        /// <param name="shelfType"></param>
        /// <param name="startShelfNo"></param>
        /// <param name="endShelfNo"></param>
        /// <param name="instrumentName"></param>
        /// <param name="instrumentCode"></param>
        /// <param name="excludeAreaCode"></param>
        /// <returns></returns>
        List<dtSearchInstrumentListResult> SearchInventoryInstrumentList(string officeCode, string locationCode, string areaCodeStr, string shelfType, string startShelfNo, string endShelfNo, string instrumentName, string instrumentCode, string[] excludeAreaCode = null);

        /// <summary>
        /// Get moving average price by condition.
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strContractCode"></param>
        /// <param name="strProjectCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <param name="strLocationCode"></param>
        /// <param name="strLotNo"></param>
        /// <returns></returns>
        doCalPriceCondition GetMovingAveragePriceCondition(string strOfficeCode, string strContractCode, string strProjectCode, string strInstrumentCode, string[] strLocationCode, string strLotNo);

        /// <summary>
        /// Get price of instrument by first-in first-out.
        /// </summary>
        /// <param name="strTransferQty"></param>
        /// <param name="strOfficeCode"></param>
        /// <param name="strLocationCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <param name="strPrevInstrumentQty"></param>
        /// <returns></returns>
        doFIFOInstrumentPrice GetFIFOInstrumentPrice(int? strTransferQty, string strOfficeCode, string strLocationCode, string strInstrumentCode, int? strPrevInstrumentQty);

        /// <summary>
        /// Register transfer instrument data on inventory DB.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        string RegisterTransferInstrument(doRegisterTransferInstrumentData Cond);

        /// <summary>
        /// For Clear Qty in All lot in tbt_accountInstalled.
        /// </summary>
        /// <param name="doClear"></param>
        /// <returns></returns>
        doClearQtyAllLot ClearQtyInAllLot(doClearQtyAllLot doClear);

        /// <summary>
        /// Check is in slip has new instrument.
        /// </summary>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        int CheckNewInstrument(string slipNo);

        /// <summary>
        /// Get new instrument in inventory slip.
        /// </summary>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<doGroupNewInstrument> GetGroupNewInstrument(string slipNo);

        /// <summary>
        /// Check is in slip has second hand instrument.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        int CheckSecondhandInstrument(string strInventorySlipNo);

        /// <summary>
        /// Get second hand instrument in inventory slip.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        List<doGroupSecondhandInstrument> GetGroupSecondhandInstrument(string strInventorySlipNo);

        /// <summary>
        /// Get FirstIn-FirstOut instrument.
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strLocationCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <returns></returns>
        List<doFIFOInstrument> GetFIFOInstrument(string strOfficeCode, string strLocationCode, string strInstrumentCode);

        /// <summary>
        /// For update transfer account data of second hand instrument.
        /// </summary>
        /// <param name="doGroupSecondhand"></param>
        /// <returns></returns>
        bool UpdateAccountTransferSecondhandInstrument(doGroupSecondhandInstrument doGroupSecondhand);

        /// <summary>
        /// Check is in slip has sample instrument.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        int CheckSampleInstrument(string strInventorySlipNo);

        /// <summary>
        /// Get sample instrument in inventory slip.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        List<doGroupSampleInstrument> GetGroupSampleInstrument(string strInventorySlipNo);

        /// <summary>
        /// Insert transferring transaction to Account Stock Moving table.
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        List<tbt_AccountStockMoving> InsertAccountStockMoving(List<tbt_AccountStockMoving> lst);

        /// <summary>
        /// Get all inventory office.
        /// </summary>
        /// <returns></returns>
        List<doOffice> GetInventoryOffice();

        List<doOffice> GetAuthorityOffice(string EmpNo);

        /// <summary>
        /// To search current inventoy instrument.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        List<dtSearchInstrumentListResult> SearchInventoryInstrumentList(doSearchInstrumentListCondition Cond);

        /// <summary>
        /// To search current inventoy instrument in screen IVS190.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        List<dtSearchInstrumentListResult> SearchInventoryInstrumentListIVS190(doSearchInstrumentListConditionIVS190 Cond);

        /// <summary>
        /// Register receive transferring instrument.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <param name="strMemo"></param>
        /// <returns></returns>
        bool RegisterReceiveInstrument(string strInventorySlipNo, string strMemo, string strApproveNo = null);

        /// <summary>
        /// To search current inventoy instrument.
        /// </summary>
        /// <param name="doCondition"></param>
        /// <returns></returns>
        List<dtSearchInstrumentListResult> SearchInventoryInstrumentListAllShelf(doSearchInstrumentListCondition doCondition);

        List<Nullable<bool>> CheckExistSummaryAsset(string strYearMonth);

        /// <summary>
        /// Search inventory slip.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        List<doInventorySlip> SearchInventorySlip(string strInventorySlipNo);

        /// <summary>
        /// Get detail of inventory slip.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        List<doInventorySlipDetail> SearchInventorySlipDetail(string strInventorySlipNo);

        List<Nullable<bool>> CheckExistSummaryInOutHeadOffice(DateTime? checkDate);

        /// <summary>
        /// Check freezed temp data.
        /// </summary>
        /// <returns></returns>
        int CheckFreezedData();

        /// <summary>
        /// Check started stock checking.
        /// </summary>
        /// <returns></returns>
        int CheckStartedStockChecking();

        /// <summary>
        /// Get stock-out instrument data for complete installation.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <param name="intTotalStockoutQty"></param>
        /// <param name="c_INV_AREA_NEW_RENTAL"></param>
        /// <param name="c_INV_AREA_NEW_SAMPLE"></param>
        /// <param name="c_INV_AREA_SE_RENTAL"></param>
        /// <param name="c_INV_LOC_WIP"></param>
        /// <returns></returns>
        List<doCompleteStockoutInstrument> GetCompleteStockoutInstrument(string strContractCode, string strInstrumentCode, Nullable<int> intTotalStockoutQty, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_SAMPLE, string c_INV_AREA_SE_RENTAL, string c_INV_LOC_WIP);

        /// <summary>
        /// Generate lot no.
        /// </summary>
        /// <param name="lotRunningNo"></param>
        /// <returns></returns>
        string GenerateLotNo(tbs_LotRunningNo lotRunningNo);

        /// <summary>
        /// Get inventory slip for returned from installation.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        List<doResultGetReturnSlip> GetReturnedSlip(string strInstallationSlipNo);

        /// <summary>
        /// Get inventory slip for returned from installation.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        List<doResultReturnInstrument> GetReturnInstrumentByInstallationSlip(string strInstallationSlipNo);

        /// <summary>
        /// Get inventory slip and map to master for view.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        List<doTbt_InventorySlipDetailForView> GetTbt_InventorySlipDetailForView(string strInventorySlipNo);

        /// <summary>
        /// Get list of inventory slip.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        List<tbt_InventorySlip> GetTbt_InventorySlipForReceiveReturn(string strInstallationSlipNo);

        /// <summary>
        /// Getting purchase order detail for maintain.
        /// </summary>
        /// <param name="strPurchaseOrder"></param>
        /// <returns></returns>
        List<doPurchaseOrderDetail> GetPurchaseOrderDetailForMaintain(string strPurchaseOrder);

        List<tbt_PurchaseOrderDetail> DeleteTbt_PurchaseOrderDetail(string purchaseOrderNo);
        List<tbt_PurchaseOrder> DeleteTbt_PurchaseOrder(string purchaseOrderNo);

        /// <summary>
        /// Get contact start service detail from Unoperated location.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doContractUnoperatedInstrument> GetContractUnoperatedInstrument(string strContractCode);

        /// <summary>
        /// To get normal shelf for input area.
        /// </summary>
        /// <param name="strAreaCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <returns></returns>
        doGetShelfOfArea GetShelfOfArea(string strAreaCode, string strInstrumentCode);

        //List<dtBatchProcessResult> GenerateSummaryInventoryInOutReport(DateTime? batchDate);
        //List<dtBatchProcessResult> GenerateInventorySummaryAsset(string strEmpNo, DateTime dtDateTime);

        #region IVP030

        /// <summary>
        /// Update inventory data from complete installation.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool UpdateCompleteInstallation(string strInstallationSlipNo, string strContractCode);

        /// <summary>
        /// Update contract start service.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool UpdateContractStartService(string strContractCode);

        /// <summary>
        /// Update complete project.
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        bool UpdateCompleteProject(string strProjectCode);

        /// <summary>
        /// Update customer acceptance for sale contract.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="dtpAcceptanceDate"></param>
        /// <returns></returns>
        bool UpdateCustomerAcceptance(string strContractCode, string strOCC, DateTime? dtpAcceptanceDate);

        /// <summary>
        /// Update complete installation but no customer acceptance for sale contract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="dtpAcceptanceDate"></param>
        /// <returns></returns>
        bool UpdateReturnSaleInstrument(string strContractCode, string strOCC, DateTime? dtpAcceptanceDate); //Add by Jutarat A. on 11042013

        /// <summary>
        /// Update cancel installation.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strInstallationSlipNo"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <returns></returns>
        bool UpdateCancelInstallation(string strContractCode, string strInstallationSlipNo, string strServiceTypeCode, bool isPartialOut = false);

        /// <summary>
        /// Update contract change instrument qty by real investigation.
        /// </summary>
        /// <param name="quotationInstrumentDetailList"></param>
        /// <returns></returns>
        bool UpdateRealInvestigation(string strContractCode, List<tbt_QuotationInstrumentDetails> quotationInstrumentDetailList);

        #endregion

        //
        /// <summary>
        /// IVP110 : Complete stock checking by auto.
        /// </summary>
        /// <returns></returns>
        doBatchProcessResult AutoCompleteStockChecking();

        /// <summary>
        /// Get shelf information on current inventory data.
        /// </summary>
        /// <param name="doGetShelf"></param>
        /// <returns></returns>
        List<doShelfCurrentData> GetShelfCurrentData(doGetShelfCurrentData doGetShelf);

        /// <summary>
        /// Get shelf information for checking.
        /// </summary>
        /// <param name="doGetShelf"></param>
        /// <returns></returns>
        List<doShelfCurrentData> GetShelfForChecking(doGetShelfCurrentData doGetShelf);

        /// <summary>
        /// Check updated cancel installation already.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        bool CheckUpdatedCancelInstallation(string strInstallationSlipNo);

        /// <summary>
        /// Check instrument exists in Inventory DB.
        /// </summary>
        /// <param name="strInstrumentCode"></param>
        /// <returns></returns>
        bool CheckExistInstrumentInInventory(string strInstrumentCode);

        /// <summary>
        /// Check whether sale instrument has already been returned
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        bool CheckExistReturnSaleInstrument(string @strInstallationSlipNo); //Add by Jutarat A. on 11042013

        List<tbt_PurchaseOrderDetail> UpdateTbt_PurchaseOrderDetail(List<tbt_PurchaseOrderDetail> lst);

        /// <summary>
        /// Check updated user acceptance already.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="acceptanceDate"></param>
        /// <returns></returns>
        bool CheckUpdatedUserAcceptance(string contractCode, DateTime? acceptanceDate);

        /// <summary>
        /// Get saled instrument detail.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<doSaleInstrument> GetSaleInstrument(string strContractCode, string strOCC);

        List<tbt_AccountInstock> GetTbt_AccountInStock(string instrumentCode, string locationCode, string officecode);
        List<tbt_AccountStockMoving> InsertTbt_AccountStockMoving(List<tbt_AccountStockMoving> lst);

        /// <summary>
        /// Get data for screen IVS200.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doResultIVS200> GetIVS200(doGetIVS200 cond);

        List<doInventoryBookingDetail> GetIVS200_Detail(doGetIVS200_Detail cond);

        /// <summary>
        /// Get installation slip data for stock-out.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<doResultInstallationSlipForStockOut> GetInstallationSlipForStockOut(doGetInstallationSlipForStockOut param);

        /// <summary>
        /// Get installation instrument data for stock-out.
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<doResultInstallationDetailForStockOut> GetInstallationDetailForStockOut(string strSlipNo);

        /// <summary>
        /// Get installation instrument data for checking.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<doResultInstallationStockOutForChecking> GetInstallationStockOutForChecking(doGetInstallationStockOutForChecking param);

        /// <summary>
        /// Get last inventory checking schedule.
        /// </summary>
        /// <returns></returns>
        List<tbt_InventoryCheckingSchedule> GetLastCheckingSchedule();

        /// <summary>
        /// Get checking status list.
        /// </summary>
        /// <param name="pCheckingYear"></param>
        /// <param name="pC_INV_CHECKING_STATUS"></param>
        /// <returns></returns>
        List<dtCheckingStatusList> GetCheckingStatusList(string pCheckingYear, string pC_INV_CHECKING_STATUS);

        /// <summary>
        /// Get office checking list.
        /// </summary>
        /// <param name="pC_INV_LOC"></param>
        /// <returns></returns>
        List<dtOfficeCheckingList> GetOfficeCheckingList(string pC_INV_LOC);

        /// <summary>
        /// Get checking year.
        /// </summary>
        /// <returns></returns>
        List<dtCheckingYear> GetCheckingYear();

        List<tbt_InventoryCheckingSchedule> UpdateTbt_InventoryCheckingSchedule(string xml);
        List<tbt_InventoryCheckingSchedule> InsertTbt_InventoryCheckingSchedule(string xml);
        List<tbt_InventoryCheckingSchedule> GetTbt_InventoryCheckingSchedule(string strYearMonth);

        /// <summary>
        /// Insert new depreviation data.
        /// </summary>
        /// <param name="insertDepreciation"></param>
        /// <returns></returns>
        string InsertDepreciationData(doInsertDepreciationData insertDepreciation);

        /// <summary>
        /// Get new moving no.
        /// </summary>
        /// <returns></returns>
        int GetMovingNumber();
        
        List<tbt_InventoryBooking> GetTbt_InventoryBooking(string strContractCode);

        /// <summary>
        /// Update stock-out instrument.
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        doBooking UpdateStockOutInstrument(doBooking booking);

        List<tbt_InventoryBookingDetail> GetTbt_InventoryBookingDetail(string strContractCode, string strInstrumentCode);

        /// <summary>
        /// Get installation slip data for partial stock-out.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<doResultInstallationSlipForStockOut> GetInstallationSlipForPartialStockOut(doGetInstallationSlipForStockOut param);

        /// <summary>
        /// Get sum of partial stock-out by contract.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doResultGetSumPartialStockOutList> GetSumPartialStockOutList(string strContractCode);

        #region IVP010

        /// <summary>
        /// New booking.
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        bool NewBooking(doBooking booking);

        /// <summary>
        /// Change expected start service date.
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        bool ChangeExpectedStartServiceDate(doBooking booking);

        /// <summary>
        /// For cancel booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        bool CancelBooking(doBooking booking);

        bool Rebooking(doBooking booking);
        #endregion

        List<dtCheckingDetailList> GetCheckingDetailList(doGetCheckingDetailList cond);

        /// <summary>
        /// For get stock checking data.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtStockCheckingList> GetStockCheckingList(doGetStockCheckingList cond);

        List<tbt_InventoryCheckingSlip> InsertTbt_InventoryCheckingSlip(List<tbt_InventoryCheckingSlip> lstInvChkSlip, bool blnLogging = true);
        List<tbt_InventoryCheckingSlipDetail> InsertTbt_InventoryCheckingSlipDetail(List<tbt_InventoryCheckingSlipDetail> lstInvChkSlipDtl, bool blnLogging = true);

        /// <summary>
        /// Get data for view in IVS201.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<doResultIVS201> GetIVS201(doGetIVS201 param);

        /// <summary>
        /// Get data for view in IVS220.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtResultIVS220> GetIVS220(doGetIVS220 param);

        /// <summary>
        /// Get inventory slip for screen IVS230.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtResultInventorySlipIVS230> GetInventorySlipIVS230(doGetInventorySlipIVS230 param);

        /// <summary>
        /// Get detail of inventory slip.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        List<dtResultInventorySlipDetail> GetInventorySlipDetail(string strInventorySlipNo);

        /// <summary>
        /// Search installation slip.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtSearchInstallationSlipResult> SearchInstallationSlip(doSearchInstallationSlipCond param);

        /// <summary>
        /// Get stock out by installation slip number.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        List<dtStockOutByInstallationSlipResult> GetStockOutByInstallationSlip(string strInstallationSlipNo);

        /// <summary>
        /// Generate picking list number.
        /// </summary>
        /// <returns></returns>
        string GeneratePickingListNo();

        /// <summary>
        /// Check can delete shelf.
        /// </summary>
        /// <param name="ShelfNo"></param>
        /// <returns></returns>
        bool IsEmptyShelf(string ShelfNo);

        /// <summary>
        /// Get project information.
        /// </summary>
        /// <param name="pProjectCode"></param>
        /// <returns></returns>
        List<doProjectInformation> GetProjectInformation(string pProjectCode);

        /// <summary>
        /// Generate inventory account.
        /// </summary>
        /// <param name="strExecUserID"></param>
        /// <param name="dtBatchDate"></param>
        /// <returns></returns>
        doBatchProcessResult GenerateInventoryAccountData(string strExecUserID, DateTime dtBatchDate);

        List<tbt_InventoryProjectWIP> InsertTbt_InventoryProjectWIP(List<tbt_InventoryProjectWIP> lstNewData);
        List<tbt_InventoryProjectWIP> UpdateTbt_InventoryProjectWIP(List<tbt_InventoryProjectWIP> lstNewData);
        List<tbt_InventoryProjectWIP> GetTbt_InventoryProjectWIP(string pProjectCode, string pAreaCode, string pInstrumentCode);

        List<tbt_InventorySlipDetail> UpdateTbt_InventorySlipDetail(List<tbt_InventorySlipDetail> lstSlip);
        List<tbt_InventorySlipDetail> UpdateTbt_InventorySlipDetail_NoLog(List<tbt_InventorySlipDetail> lstSlip);

        /// <summary>
        /// Check instrument exists in tbm_InstrumentExpansion.
        /// </summary>
        /// <param name="strInstrumentCode"></param>
        /// <returns></returns>
        bool CheckInstrumentExpansion(string strInstrumentCode);

        int CheckImplementStockChecking();

        List<tbt_InventorySlipDetail> GetTbt_InventorySlipDetail(string slipNo);
        List<tbt_InventorySlipDetail> GetTbt_InventorySlipDetail(string slipNo, Int32? RunningNo);

        List<tbt_AccountSampleInstock> GetTbt_AccountSampleInStock(string instrumentCode, string locationCode, string officeCode);

        /// <summary>
        /// For update transfer account data of second hand instrument in screen IVS180.
        /// </summary>
        /// <param name="doGroupSecondhand"></param>
        /// <returns></returns>
        bool UpdateAccountTransferSecondhandInstrumentIVS180(doGroupSecondhandInstrument doGroupSecondhand);

        /// <summary>
        /// For update transfer account data of second hand instrument in screen IVS190.
        /// </summary>
        /// <param name="doGroupSecondhand"></param>
        /// <returns></returns>
        bool UpdateAccountTransferSecondhandInstrumentIVS190(doGroupSecondhandInstrument doGroupSecondhand);

        List<tbt_InventoryCurrent> DeleteTbt_InventoryCurrent(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode);
        List<tbt_InventoryCurrent> DeleteTbt_InventoryCurrent_NoLog(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode);

        /// <summary>
        /// Update complete stock out for partial.
        /// </summary>
        /// <param name="doComplete"></param>
        /// <returns></returns>
        bool UpdateCompleteStockoutForPartial(doUpdateCompleteStockoutForPartial doComplete);
        
        bool CheckTransferFromBuffer(string strLocationCode, string strInstrumentCode);

        /// <summary>
        /// Get normal shelf by shelf number that exist in inventory curren.
        /// </summary>
        /// <param name="ShelfNo"></param>
        /// <returns></returns>
        doNormalShelfExistCurrent GetNormalShelfExistCurrent(string ShelfNo);

        #region R2

        List<tbt_InventoryCheckingTemp> GetTbt_InventoryCheckingTemp(string strCheckingYearMonth, string strLocationCode, string strOfficeCode, string strShelfNo, string strAreaCode, string strInstrumentCode);
        List<tbt_InventoryCheckingTemp> InsertTbt_InventoryCheckingTemp(List<tbt_InventoryCheckingTemp> lstCheckingTemp);
        List<tbt_InventoryCheckingTemp> UpdateTbt_InventoryCheckingTemp(List<tbt_InventoryCheckingTemp> lstCheckingTemp);

        #endregion

        /// <summary>
        /// Get price of instrument by last-in first-out.
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strLocationCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <param name="intTransferQty"></param>
        /// <param name="intPrevInstrumentQty"></param>
        /// <returns></returns>
        doLIFOInstrumentPrice GetLIFOInstrumentPrice(string strOfficeCode, string strLocationCode, string strInstrumentCode, int? intTransferQty, int? intPrevInstrumentQty, string c_CURRENCY_LOCAL, string c_CURRENCY_US);

        /// <summary>
        /// Update complete stock-out for sales Maintenance.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool UpdateCompleteSaleMA(string strInstallationSlipNo, string strContractCode);

        /// <summary>
        /// Get instrument data for complete sales maintenance.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doInstrumentMASale> GetInstrumentForCompleteMASale(string strContractCode);

        /// <summary>
        /// Update all contract's inventory slip status to complete.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<tbt_InventorySlip> UpdatePartialToCompleteStatus(string strContractCode);

        /// <summary>
        /// Get In-Report header data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtInReportHeader> GetStockReport_InReport_Header(doIVS280SearchCondition param);
        
        /// <summary>
        /// Get In-Report detail data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtInReportDetail> GetStockReport_InReport_Detail(string reportType, string slipNo);

        /// <summary>
        /// Get Out-Report header data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtOutReportHeader> GetStockReport_OutReport_Header(doIVS281SearchCondition param);

        /// <summary>
        /// Get Out-Report detail data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtOutReportDetail> GetStockReport_OutReport_Detail(string reportType, string slipNo);

        /// <summary>
        /// Get Return-Report header data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtReturnReportHeader> GetStockReport_ReturnReport_Header(doIVS282SearchCondition param);

        /// <summary>
        /// Get Return-Report detail data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtReturnReportDetail> GetStockReport_ReturnReport_Detail(string reportType, string slipNo);

        /// <summary>
        /// Get Inprocess-to-Install Report data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtInprocessToInstallReport> GetStockReport_InprocessToInstall(doIVS284SearchCondition param);

        /// <summary>
        /// Get Inprocess-to-Install Report detail data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtInprocessToInstallReportDetail> GetStockReport_InprocessToInstall_Detail(doIVS284SearchCondition param);


        /// <summary>
        /// Get Physical Report data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<dtPhysicalReport> GetStockReport_Physical(string reportType, string instrumentCode, string yearMonth);

        /// <summary>
        /// Get Inprocess Report data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="contractCode"></param>
        /// <param name="processDate"></param>
        /// <returns></returns>
        List<dtInProcessReport> GetStockReport_InProcess(string reportType, string contractCode, string processDate);

        /// <summary>
        /// Get Inprocess Report data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtInProcessReport> GetStockReport_InProcess(doIVS286SearchCondition param);

        /// <summary>
        /// Get Inprocess Report Detail data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtInProcessReportDetail> GetStockReport_InProcess_Detail(doIVS286SearchCondition param);

        /// <summary>
        /// Get Stock List Report data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<dtStockListReport> GetStockReport_StockList(string reportType, string instrumentCode);

        /// <summary>
        /// Get Instrument List data for Movement Report
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<dtInstrumentForMovementReport> GetStockReport_Instrument(string reportType, string instrumentCode);

        /// <summary>
        /// Get Movement List Report data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<dtMovementReport> GetStockReport_Movement(string reportType, string instrumentCode, string yearMonth);

        /// <summary>
        /// Get Change Area Report header data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtChangeAreaReportHeader> GetStockReport_ChangeArea_Header(doIVS288SearchCondition param);

        /// <summary>
        /// Get Change Area detail data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtChangeAreaReportDetail> GetStockReport_ChangeArea_Detail(string slipNo);

        /// <summary>
        /// Get Elimination Report header data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtEliminateReportHeader> GetStockReport_Eliminate_Header(doIVS289SearchCondition param);

        /// <summary>
        /// Get Elimination detail data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtEliminateReportDetail> GetStockReport_Eliminate_Detail(string slipNo);

        /// <summary>
        /// Get Buffer Loss Report header data
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<dtBufferLossReportHeader> GetStockReport_BufferLoss_Header(doIVS290SearchCondition param);

        /// <summary>
        /// Get Buffer Loss detail data
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtBufferLossReportDetail> GetStockReport_BufferLoss_Detail(string slipNo);

        /// <summary>
        /// Delete inventory booking
        /// </summary>
        /// <param name="strContractCode"></param>
        void DeleteTbt_InventoryBookingWithLog(string strContractCode);

        /// <summary>
        /// Delete inventory booking detail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strInstrumentCode"></param>
        void DeleteTbt_InventoryBookingDetailWithLog(string strContractCode, string strInstrumentCode);
        
        /// <summary>
        /// Update inventory booking detail
        /// </summary>
        /// <param name="lstBookingDtl"></param>
        void UpdateTbt_InventoryBookingDetail(List<tbt_InventoryBookingDetail> lstBookingDtl);

        /// <summary>
        /// Insert inventory booking detail
        /// </summary>
        /// <param name="lstBookingDtl"></param>
        void InsertTbt_InventoryBookingDetail(List<tbt_InventoryBookingDetail> lstBookingDtl);
    
        /// <summary>
        /// Get account in-process
        /// </summary>
        /// <param name="locationCode"></param>
        /// <param name="contractCode"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<tbt_AccountInprocess> GetTbt_AccountInProcess(string locationCode, string contractCode, string instrumentCode);

        string GenerateVoucherID(DateTime? stockInDate);

        List<doSearchReceiveSlipResult> SearchReceiveSlip(doIVS030SearchCondition param);

        #region Monthly Price @ 2015
        decimal GetMonthlyAveragePrice(string instrumentCode, DateTime? yearmonth, string accountCode, string c_CURRENCY_LOCAL, string c_CURRENCY_US);
        doBatchProcessResult UpdateMonthlyAveragePrice(string strUserId, DateTime? updateDate, DateTime dtBatchDate);
        #endregion

        /// <summary>
        /// Get period list for Inprocess report.
        /// </summary>
        /// <returns></returns>
        List<string> GetStockReport_InProcessPeriod();

        /// <summary>
        /// Get period list for Physical report.
        /// </summary>
        /// <returns></returns>
        List<string> GetStockReport_PhysicalPeriod();

        /// <summary>
        /// Get business date by input offset
        /// </summary>
        /// <param name="date"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        DateTime? GetBusinessDateByOffset(DateTime? date, int offset);

        /// <summary>
        /// Get next business date by offset from tbs_Configuration
        /// </summary>
        /// <param name="referencedate"></param>
        /// <returns></returns>
        DateTime? GetPreviousBusinessDateByDefaultOffset(DateTime? referencedate);

        /// <summary>
        /// Get previous business date by offset from tbs_Configuration
        /// </summary>
        /// <param name="referencedate"></param>
        /// <returns></returns>
        DateTime? GetNextBusinessDateByDefaultOffset(DateTime? referencedate);
    }
}
