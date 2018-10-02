namespace SECOM_AJIS.DataEntity.Inventory
{
	partial class BizIVDataEntities
	{
		/// \fn public virtual List<Nullable<bool>> CheckExistInstrument(string strInstrumentCode)
		/// \brief (Call stored procedure: sp_IV_CheckExistInstrument).

		/// \fn public virtual List<Nullable<bool>> CheckExistLotRunningNumber(string instrumentCode, string depreciationPeriodForContract, string startYearMonth)
		/// \brief (Call stored procedure: sp_IV_CheckExistLotRunningNumber).

		/// \fn public virtual List<Nullable<bool>> CheckExistSummaryAsset(string strYearMonth)
		/// \brief (Call stored procedure: sp_IV_CheckExistSummaryAsset).

		/// \fn public virtual List<Nullable<bool>> CheckExistSummaryInOutHeadOffice(Nullable<System.DateTime> checkDate)
		/// \brief (Call stored procedure: sp_IV_CheckExistSummaryInOutHeadOffice).

		/// \fn public virtual List<Nullable<int>> CheckFreezedData()
		/// \brief (Call stored procedure: sp_IV_CheckFreezedData).

		/// \fn public virtual List<Nullable<int>> CheckImplementStockChecking()
		/// \brief (Call stored procedure: sp_IV_CheckImplementStockChecking).

		/// \fn public virtual List<Nullable<int>> CheckNewInstrument(string slipNo, string c_INV_AREA_NEW_SALE, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_DEMO)
		/// \brief (Call stored procedure: sp_IV_CheckNewInstrument).

		/// \fn public virtual List<Nullable<int>> CheckSampleInstrument(string strInventorySlipNo, string c_INV_AREA_NEW_SAMPLE)
		/// \brief (Call stored procedure: sp_IV_CheckSampleInstrument).

		/// \fn public virtual List<Nullable<int>> CheckSecondhandInstrument(string strInventorySlipNo, string c_INV_AREA_SE_RENTAL, string c_INV_AREA_SE_LENDING_DEMO, string c_INV_AREA_SE_HANDLING_DEMO)
		/// \brief (Call stored procedure: sp_IV_CheckSecondhandInstrument).

		/// \fn public virtual List<Nullable<int>> CheckStartedStockChecking()
		/// \brief (Call stored procedure: sp_IV_CheckStartedStockChecking).

		/// \fn public virtual List<Nullable<int>> CheckTransferFromBuffer(string locationCode, string instrumentCode, string c_INV_LOC_BUFFER, string c_INV_AREA_SE_RENTAL, string c_INV_AREA_SE_HANDLING_DEMO, string c_INV_AREA_SE_LENDLING_DEMO)
		/// \brief (Call stored procedure: sp_IV_CheckTransferFromBuffer).

		/// \fn public virtual List<Nullable<bool>> CheckUpdatedCancelInstallation(string strInstallationSlipNo, string c_INV_TRANSFERTYPE_CANCEL_INSTALLATION)
		/// \brief (Call stored procedure: sp_IV_CheckUpdatedCancelInstallation).

		/// \fn public virtual List<Nullable<bool>> CheckUpdatedUserAcceptance(string strContractCode, Nullable<System.DateTime> dtpAcceptanceDate, string c_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE)
		/// \brief (Call stored procedure: sp_IV_CheckUpdatedUserAcceptance).

		/// \fn public virtual List<tbt_AccountInstalled> ClearQtyInAllLot(string officeCode, string locationCode, string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_ClearQtyInAllLot).

		/// \fn public virtual List<tbt_InventoryBooking> DeleteTbt_InventoryBooking(string pContractCode)
		/// \brief (Call stored procedure: Sp_IV_DeleteTbt_InventoryBooking).

		/// \fn public virtual List<tbt_InventoryBookingDetail> DeleteTbt_InventoryBookingDetail(string pContractCode, string pInstrumentCode)
		/// \brief (Call stored procedure: Sp_IV_DeleteTbt_InventoryBookingDetail).

		/// \fn public virtual List<tbt_InventoryCurrent> DeleteTbt_InventoryCurrent(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_DeleteTbt_InventoryCurrent).

		/// \fn public virtual List<tbt_InventorySlip> DeleteTbt_InventorySlip(string slipNo)
		/// \brief (Call stored procedure: sp_IV_DeleteTbt_InventorySlip).

		/// \fn public virtual List<tbt_InventorySlipDetail> DeleteTbt_InventorySlipDetail(string slipNo)
		/// \brief (Call stored procedure: sp_IV_DeleteTbt_InventorySlipDetail).

		/// \fn public virtual List<tbt_PurchaseOrder> DeleteTbt_PurchaseOrder(string purchaseOrderNo)
		/// \brief (Call stored procedure: sp_IV_DeleteTbt_PurchaseOrder).

		/// \fn public virtual List<tbt_PurchaseOrderDetail> DeleteTbt_PurchaseOrderDetail(string purchaseOrderNo)
		/// \brief (Call stored procedure: sp_IV_DeleteTbt_PurchaseOrderDetail).

		/// \fn public virtual List<doCSVMovingAssetAcc> ExportMovingAssetAcc(string pC_INV_ACCOUNT_CODE)
		/// \brief (Call stored procedure: sp_IV_ExportMovingAssetAcc).

		/// \fn public virtual int FreezeInstrumentDataForStockCheckingProcess(string pCheckingYearMonth, string pC_INV_LOC_INSTOCK, string pC_INV_LOC_PRE_ELIMINATION, string pC_INV_LOC_REPAIRING, Nullable<bool> pC_FLAG_OFF, string pC_INV_CHECKING_STATUS_PREPARING, string pC_INV_SHELF_NO_NOT_PRICE)
		/// \brief (Call stored procedure: Sp_IV_FreezeInstrumentDataForStockCheckingProcess).

		/// \fn public virtual List<string> GenerateInventorySlipNo(Nullable<int> c_INV_SLIP_NO_MAXIMUM, Nullable<int> c_INV_SLIP_NO_MINIMUM, string officeCode, string slipid)
		/// \brief (Call stored procedure: SP_IV_GenerateInventorySlipNo).

		/// \fn public virtual List<dtBatchProcessResult> GenerateInventorySummaryAsset(string strYearMonth, string c_FUNC_LOGISTIC_HQ, string c_INV_OFFICE_SNR, string c_INV_TRANSFERTYPE_STOCKIN_PURCHASE, string c_INV_TRANSFERTYPE_STOCKIN_SPECIAL, string c_INV_TRANSFERTYPE_TRANSFER_OFFICE, string c_INV_AREA_NEW_SAMPLE, string c_INV_AREA_NEW_SALE, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_DEMO, string c_INV_AREA_SE_RENTAL, string c_INV_AREA_SE_LENDING_DEMO, string c_INV_AREA_SE_HANDLING_DEMO, string c_INV_SLIP_STATUS_COMPLETE, string c_INV_SLIP_STATUS_TRANSFER, string c_INV_TRANSFERTYPE_COMPLETE_AFTER_START, string c_INV_TRANSFERTYPE_COMPLETE_BEFORE_START, string c_INV_TRANSFERTYPE_COMPLETE_MK30, string c_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20, string c_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20, string c_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03, string c_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03, string c_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29, string c_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29, string c_INV_TRANSFERTYPE_STOCKOUT_SPECIAL, string c_INV_TRANSFERTYPE_START_SERVICE, string c_INV_TRANSFERTYPE_PRE_ELIMINATION, string c_INV_TRANSFERTYPE_CANCEL_INSTALLATION, string c_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE, string c_INV_TRANSFERTYPE_CHECKING_RETURNED, string c_INV_TRANSFERTYPE_REPAIR_REQUEST, string c_INV_TRANSFERTYPE_REPAIR_RETURN, string c_INV_TRANSFERTYPE_TRANSFER_BUFFER, string c_INV_TRANSFERTYPE_CHANGE_INVESTIGATION, string c_SALE_INSTALL_TYPE_ADD, string c_SALE_INSTALL_TYPE_NEW, string c_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW, string c_RENTAL_INSTALL_TYPE_NEW, string c_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW, string c_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE, string c_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE, string c_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP, string c_INV_LOC_WAITING_RETURN, string c_INV_LOC_RETURNED, string c_INV_LOC_INSTOCK, string c_INV_LOC_REPAIRING, string c_INV_LOC_BUFFER, string c_INV_LOC_USER, Nullable<bool> c_FLAG_OFF, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_IV_GenerateInventorySummaryAsset).

		/// \fn public virtual List<string> GenerateLotNo(Nullable<int> c_INV_LOT_NO_MAXIMUM, Nullable<int> c_INV_LOT_NO_MINIMUM, string instrumentCode, string depreciationPeriodForContract, string startYearMonth)
		/// \brief (Call stored procedure: SP_IV_GenerateLotNo).

		/// \fn public virtual List<string> GeneratePickingListNo(Nullable<int> pC_INV_PICKING_NO_MINIMUM, Nullable<int> pC_INV_PICKING_NO_MAXIMUM)
		/// \brief (Call stored procedure: Sp_IV_GeneratePickingListNo).

		/// \fn public virtual List<string> GeneratePurchaseOrderNo(Nullable<int> c_INV_SLIP_NO_MAXIMUM, Nullable<int> c_INV_SLIP_NO_MINIMUM, string nationCodeCode)
		/// \brief (Call stored procedure: SP_IV_GeneratePurchaseOrderNo).

		/// \fn public virtual List<dtBatchProcessResult> GenerateSummaryInventoryInOutReport(Nullable<System.DateTime> tRANSFERMONTH, Nullable<System.DateTime> dateBatchDate, string c_INV_LOC_BUFFER, string c_INV_LOC_INSTOCK, string c_INV_LOC_PRE_ELIMINATION, string c_INV_LOC_REPAIR_REQUEST, string c_INV_LOC_REPAIR_RETURN, string c_INV_LOC_RETURN_WIP, string c_INV_LOC_RETURNED, string c_INV_LOC_SPECIAL, string c_INV_LOC_TRANSFER, string c_INV_LOC_UNOPERATED_WIP, string c_INV_LOC_USER, string c_INV_LOC_WAITING_RETURN, string c_INV_LOC_WIP, string c_INV_SLIP_STATUS_COMPLETE, string c_INV_TRANSFERTYPE_CANCEL_INSTALLATION, string c_INV_TRANSFERTYPE_CHANGE_INVESTIGATION, string c_INV_TRANSFERTYPE_COMPLETE_AFTER_START, string c_INV_TRANSFERTYPE_COMPLETE_BEFORE_START, string c_INV_TRANSFERTYPE_COMPLETE_PROJECT, string c_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE, string c_INV_TRANSFERTYPE_FIX_ADJUSTMENT, string c_INV_TRANSFERTYPE_PRE_ELIMINATION, string c_INV_TRANSFERTYPE_RECEIVE_RETURNED, string c_INV_TRANSFERTYPE_REPAIR_REQUEST, string c_INV_TRANSFERTYPE_REPAIR_RETURN, string c_INV_TRANSFERTYPE_START_SERVICE, string c_INV_TRANSFERTYPE_STOCKIN_PURCHASE, string c_INV_TRANSFERTYPE_STOCKIN_SPECIAL, string c_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03, string c_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20, string c_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29, string c_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03, string c_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20, string c_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29, string c_INV_TRANSFERTYPE_STOCKOUT_PROJECT, string c_INV_TRANSFERTYPE_STOCKOUT_SPECIAL, string c_INV_TRANSFERTYPE_TRANSFER_BUFFER, string c_INV_TRANSFERTYPE_TRANSFER_OFFICE, string c_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW, string c_RENTAL_INSTALL_TYPE_NEW, string c_RENTAL_INSTALL_TYPE_REMOVE_ALL, string c_SALE_INSTALL_TYPE_REMOVE_ALL, Nullable<bool> c_FLAG_OFF, Nullable<bool> c_FLAG_ON, string c_INV_LOC_PROJECT_WIP, string c_INV_LOC_PARTIAL_OUT)
		/// \brief (Call stored procedure: sp_IV_GenerateSummaryInventoryInOutReport).

		/// \fn public virtual List<doOffice> GetAuthorityOffice(string empNo)
		/// \brief (Call stored procedure: sp_IV_GetAuthorityOffice).

		/// \fn public virtual List<dtCheckingDetailList> GetCheckingDetailList(string pCheckingYearMonth, string pOfficeCode, string pLocationCode, string pC_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetCheckingDetailList).

		/// \fn public virtual List<dtCheckingStatusList> GetCheckingStatusList(string pCheckingYear, string pC_INV_CHECKING_STATUS)
		/// \brief (Call stored procedure: sp_IV_GetCheckingStatusList).

		/// \fn public virtual List<doCheckingTempForGenSlip> GetCheckingTempForGenSlip(string pCheckingYearMonth)
		/// \brief (Call stored procedure: Sp_IV_GetCheckingTempForGenSlip).

		/// \fn public virtual List<dtCheckingYear> GetCheckingYear()
		/// \brief (Call stored procedure: sp_IV_GetCheckingYear).

		/// \fn public virtual List<doCompleteRemoveInstrument> GetCompleteRemoveInstrument(string strContractCode, string strInstrumentCode, Nullable<int> intTotalStockoutQty, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_SAMPLE, string c_INV_AREA_SE_RENTAL, string c_INV_LOC_UNOPERATED_WIP)
		/// \brief (Call stored procedure: sp_IV_GetCompleteRemoveInstrument).

		/// \fn public virtual List<doCompleteStockoutInstrument> GetCompleteStockoutInstrument(string strContractCode, string strInstrumentCode, Nullable<int> intTotalStockoutQty, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_SAMPLE, string c_INV_AREA_SE_RENTAL, string c_INV_LOC_WIP)
		/// \brief (Call stored procedure: sp_IV_GetCompleteStockoutInstrument).

		/// \fn public virtual List<doContractUnoperatedInstrument> GetContractUnoperatedInstrument(string strContractCode, string c_INV_TRANSFERTYPE_COMPLETE_BEFORE_START)
		/// \brief (Call stored procedure: sp_IV_GetContractUnoperatedInstrument).

		/// \fn public virtual List<doContractWIPInstrument> GetContractWIPInstrument(string strContractCode, string strServiceTypeCode, string strInstallationSlipNo, string c_SERVICE_TYPE_SALE, string c_INV_AREA_NEW_SALE, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_SAMPLE, string c_INV_AREA_SE_RENTAL, string c_INV_LOC_WIP)
		/// \brief (Call stored procedure: sp_IV_GetContractWIPInstrument).

		/// \fn public virtual List<tbt_InventoryDepreciation> GetDepreciationData(string strInstrumentCode, string strStartYearMonth, Nullable<int> intDepreciationPeriod)
		/// \brief (Call stored procedure: sp_IV_GetDepreciationData).

		/// \fn public virtual List<doCSVassetAmountAcc> GetExportAssetAmountAcc(string pC_INV_LOC_INSTOCK, string pC_INV_LOC_TRANSFER, string pC_INV_LOC_PARTIAL_OUT, string pC_INV_LOC_WIP, string pC_INV_LOC_PROJECT_WIP, string pC_INV_LOC_UNOPERATED_WIP, string pC_INV_AREA_NEW_DEMO, string pC_INV_AREA_NEW_RENTAL, string pC_INV_AREA_NEW_SALE, string pC_INV_AREA_NEW_SAMPLE, string pC_INV_AREA_SE_HANDLING_DEMO, string pC_INV_AREA_SE_LENDING_DEMO, string pC_INV_AREA_SE_RENTAL, string pC_SERVICE_TYPE_SALE, string pC_SERVICE_TYPE_RENTAL)
		/// \brief (Call stored procedure: Sp_IV_ExportAssetAmountAcc).

		/// \fn public virtual List<doCSVInvDepreciationAcc> GetExportInvDepreciationAcc()
		/// \brief (Call stored procedure: Sp_IV_ExportInvDepreciationAcc).

		/// \fn public virtual List<doFIFOInstrument> GetFIFOInstrument(string strOfficeCode, string strLocationCode, string strInstrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetFIFOInstrument).

		/// \fn public virtual List<doFIFOInstrument> GetFIFOInstrumentAbsoluteQty(string strOfficeCode, string strLocationCode, string strInstrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetFIFOInstrumentAbsoluteQty).

		/// \fn public virtual List<Nullable<decimal>> GetFIFOInstrumentPrice(Nullable<int> intTransferQty, string strOfficeCode, string strLocationCode, string strInstrumentCode, Nullable<int> intPrevInstrumentQty)
		/// \brief (Call stored procedure: sp_IV_GetFIFOInstrumentPrice).

		/// \fn public virtual List<doGroupNewInstrument> GetGroupNewInstrument(string c_INV_AREA_NEW_SALE, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_DEMO, string slipNo)
		/// \brief (Call stored procedure: sp_IV_GetGroupNewInstrument).

		/// \fn public virtual List<doGroupSampleInstrument> GetGroupSampleInstrument(string strInventorySlipNo, string c_INV_AREA_NEW_SAMPLE)
		/// \brief (Call stored procedure: sp_IV_GetGroupSampleInstrument).

		/// \fn public virtual List<doGroupSecondhandInstrument> GetGroupSecondhandInstrument(string strInventorySlipNo, string c_INV_AREA_SE_RENTAL, string c_INV_AREA_SE_HANDLING_DEMO, string c_INV_AREA_SE_LENDING_DEMO)
		/// \brief (Call stored procedure: sp_IV_GetGroupSecondhandInstrument).

		/// \fn public virtual List<doResultInstallationDetailForStockOut> GetInstallationDetailForStockOut(string pC_INV_AREA_NEW_SALE, string pC_INV_AREA_NEW_RENTAL, string pC_INV_AREA_NEW_SAMPLE, string pC_INV_AREA_SE_RENTAL, string pC_INST_TYPE_GENERAL, string pSlipNo)
		/// \brief (Call stored procedure: Sp_IV_GetInstallationDetailForStockOut).

		/// \fn public virtual List<doResultInstallationSlipForStockOut> GetInstallationSlipForPartialStockOutList(string pC_RENTAL_INSTALL_TYPE, string pC_SALE_INSTALL_TYPE, string pC_SLIP_STATUS_NOT_STOCK_OUT, string pC_CONFIG_WILDCARD, string pC_SLIP_STATUS_PARTIAL_STOCK_OUT, string pInstallationSlipNo, string pContractCode, string pContractTargerName, string pSiteName, Nullable<System.DateTime> pExpectedStockOutDateFrom, Nullable<System.DateTime> pExpectedStockOutDateTo, string pOfficeCode, string pInstallationType)
		/// \brief (Call stored procedure: Sp_IV_GetInstallationSlipForPartialStockOutList).

		/// \fn public virtual List<doResultInstallationSlipForStockOut> GetInstallationSlipForStockOut(string pC_RENTAL_INSTALL_TYPE, string pC_SALE_INSTALL_TYPE, string pC_SLIP_STATUS_NOT_STOCK_OUT, string pC_CONFIG_WILDCARD, string pInstallationSlipNo, string pProjectCode, string pContractCode, string pContractTargerName, string pSiteName, Nullable<System.DateTime> pExpectedStockOutDateFrom, Nullable<System.DateTime> pExpectedStockOutDateTo, string pOfficeCode, string pInstallationType)
		/// \brief (Call stored procedure: Sp_IV_GetInstallationSlipForStockOut).

		/// \fn public virtual List<doResultInstallationStockOutForChecking> GetInstallationStockOutForChecking(string pC_FUNC_LOGISTIC_HQ, string pC_INV_AREA_NEW_SALE, string pC_INV_AREA_NEW_RENTAL, string pC_INV_AREA_NEW_SAMPLE, string pC_INV_AREA_SE_RENTAL, string pC_INV_LOC_INSTOCK, string pC_INV_LOC_PROJECT_WIP, string pOfficeCode, string pSlipNo, string pProjectCode, string pInstrumentCode, string pSaleShelfNo, string pRentalShelfNo, string pSampleShelfNo, string pSecondShelfNo)
		/// \brief (Call stored procedure: Sp_IV_GetInstallationStockOutForChecking).

        /// \fn public List<doInstrumentMASale> GetInstrumentForCompleteMASale(string strContractCode)
        /// \brief (Call stored procedure: Sp_IV_GetInstrumentForCompleteMASale).

		/// \fn public virtual List<doOffice> GetInventoryHeadOffice(string pC_OFFICE_LOGISTIC_HEAD)
		/// \brief (Call stored procedure: sp_IV_GetInventoryHeadOffice).

		/// \fn public virtual List<doOffice> GetInventoryOffice(string c_OFFICELOGISTIC_NONE)
		/// \brief (Call stored procedure: sp_IV_GetInventoryOffice).

		/// \fn public virtual List<dtResultInventorySlipDetail> GetInventorySlipDetail(string pC_INV_AREA, string pInventorySlipNo)
		/// \brief (Call stored procedure: Sp_IV_GetInventorySlipDetail).

		/// \fn public virtual List<doInventorySlipDetailList> GetInventorySlipDetailForRegister(string slipNo, string c_INV_STOCKIN_TYPE, string c_INV_REGISTER_ASSET, string c_INV_AREA, string c_INV_REGISTER_ASSET_UNREGISTER)
		/// \brief (Call stored procedure: sp_IV_GetInventorySlipDetailForRegister).

		/// \fn public virtual List<doInventorySlipDetailList> GetInventorySlipDetailForSearch(string slipNo, string c_INV_STOCKIN_TYPE, string c_INV_REGISTER_ASSET, string c_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetInventorySlipDetailForSearch).

		/// \fn public virtual List<doInventorySlipList> GetInventorySlipForSearch(string slipNo, string purchaseOrderNo, string stockInFlag, string deliveryOrderNo, Nullable<System.DateTime> stockInDateFrom, Nullable<System.DateTime> stockInDateTo, string registerAssetFlag, string memo, string c_INV_STOCKIN_TYPE, string c_INV_REGISTER_ASSET, string c_INV_STOCKIN_TYPE_PURCHASE, string c_INV_STOCKIN_TYPE_SPECIAL)
		/// \brief (Call stored procedure: sp_IV_GetInventorySlipForSearch).

		/// \fn public virtual List<dtResultInventorySlipIVS230> GetInventorySlipIVS230(string pC_INV_LOC, string pC_INV_TRANSFERTYPE, string pC_INV_SLIP_STATUS, string pC_INV_TRANSFERTYPE_TRANSFER_AREA, string pC_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03, string pC_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20, string pC_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29, string pInventorySlipNo, string pSlipStatus, string pOfficeCode, Nullable<System.DateTime> pDateFrom, Nullable<System.DateTime> pDateTo, string pEmpNo, string pProjectCode, string pStockOutType)
		/// \brief (Call stored procedure: Sp_IV_GetInventorySlipIVS230).

		/// \fn public virtual List<doOffice> GetInventorySrinakarinOffice(string office_Code)
		/// \brief (Call stored procedure: sp_IV_GetInventorySrinakarinOffice).

		/// \fn public virtual List<doIVR> GetIVR(string pC_INV_STOCKIN_TYPE, string pC_INV_LOC, string pC_INV_AREA, string pInventorySlipNo)
		/// \brief (Call stored procedure: Sp_IV_GetIVR).

		/// \fn public virtual List<doIVR100> GetIVR100(string pC_INV_LOC, string pC_INV_AREA, string pC_CONFIG_SUSPENDFLAG, string pInventorySlipNo)
		/// \brief (Call stored procedure: Sp_IV_GetIVR100).

		/// \fn public virtual List<doIVR110> GetIVR110(string pC_INV_LOC, string pC_INV_AREA, string pC_CONFIG_SUSPENDFLAG, string pInventorySlipNo)
		/// \brief (Call stored procedure: Sp_IV_GetIVR110).

		/// \fn public virtual List<RPTdoIVR140> GetIVR140(string strInventorySlipNo, string c_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetIVR140).

		/// \fn public virtual List<RPTdoIVR141> GetIVR141(string strInventorySlipNo, string c_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetIVR141).

		/// \fn public virtual List<RPTdoIVR142> GetIVR142(string strInventorySlipNo, string c_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetIVR142).

		/// \fn public virtual List<RPTdoIVR143> GetIVR143(string strInventorySlipNo, string c_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetIVR143).

		/// \fn public virtual List<RPTdoIVR150> GetIVR150(string strYearMonth)
		/// \brief (Call stored procedure: sp_IV_GetIVR150).

		/// \fn public virtual List<doIVR170> GetIVR170(string pickingListNo, string c_INV_AREA)
		/// \brief (Call stored procedure: Sp_IV_GetIVR170).

		/// \fn public virtual List<doIVR190> GetIVR190(string purchaseOrderNo, string c_OFFICELEVEL_HEAD, string c_DEPT_PURCHASE, Nullable<bool> c_FLAG_ON, string c_CURRENCY_TYPE_THB, string c_CURRENCY_TYPE_USD, string c_CURRENCY_TYPE_EUR, string c_CURRENCY_TYPE_YEN)
		/// \brief (Call stored procedure: Sp_IV_GetIVR190).

		/// \fn public virtual List<doIVR191> GetIVR191(string purchaseOrderNo, string c_VAT_THB, string c_UNIT_PCS, string c_OFFICELEVEL_HEAD, string c_DEPT_PURCHASE, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: Sp_IV_GetIVR191).

		/// \fn public virtual List<doResultIVS200> GetIVS200(string officeCode, string c_INV_LOC_INSTOCK, string c_INV_AREA_NEW_SAMPLE, string c_INV_AREA_NEW_RENTAL, string c_INV_AREA_NEW_SALE, string instrumentCode, string instrumentName, string c_CONFIG_WILDCARD, Nullable<bool> haveOrderQty, Nullable<bool> belowSafety, Nullable<bool> minus)
		/// \brief (Call stored procedure: sp_IV_GetIVS200).

		/// \fn public virtual List<doResultIVS201> GetIVS201(string pC_INV_LOC, string pC_INV_AREA, string pC_INV_AREA_SHORT, string pC_CONFIG_WILDCARD, string pOfficeCode, string pLocationCode, string pAreaCode, string pShelfNoFrom, string pShelfNoTo, string pInstrumentCode, string pInstrumentName)
		/// \brief (Call stored procedure: Sp_IV_GetIVS201).

		/// \fn public virtual List<dtResultIVS220> GetIVS220(string pC_INV_AREA, string pC_INV_AREA_SHORT, string pC_INV_SLIP_PREFIX, string pC_INV_TRANSFERTYPE_TRANSFER_AREA, string pC_INV_TRANSFERTYPE_TRANSFER_SHELF, string pC_CONFIG_WILDCARD, string pOfficeCode, string pLocationCode, Nullable<System.DateTime> pDateFrom, Nullable<System.DateTime> pDateTo, string pInstrumentCode, string pInstrumentName, string pInventorySlipNo, string pAreaCode)
		/// \brief (Call stored procedure: Sp_IV_GetIVS220).

		/// \fn public virtual List<Nullable<System.DateTime>> GetLastBusinessDate_(Nullable<System.DateTime> date)
		/// \brief (Call stored procedure: Sp_IV_GetLastBusinessDate).

		/// \fn public virtual List<tbt_InventoryCheckingSchedule> GetLastCheckingSchedule()
		/// \brief (Call stored procedure: sp_IV_GetLastCheckingSchedule).

		/// \fn public virtual List<doFIFOInstrument> GetLIFOInstrument(string strOfficeCode, string strLocationCode, string strInstrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetLIFOInstrument).

        /// \fn public virtual List<Nullable<decimal>> GetLIFOInstrumentPrice(string strOfficeCode, string strLocationCode, string strInstrumentCode, Nullable<int> intTransferQty, Nullable<int> intPrevInstrumentQty)
        /// \brief (Call stored procedure: sp_IV_GetLIFOInstrumentPrice).

		/// \fn public virtual List<string> GetLotRunningNo(string instrumentCode, string depreciationPeriodForContract, string startYearMonth)
		/// \brief (Call stored procedure: sp_IV_GetLotRunningNo).

		/// \fn public virtual List<string> GetMaxLotRunningNo(string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetMaxLotRunningNo).

		/// \fn public virtual List<doCalPriceCondition> GetMovingAveragePriceCondition(string strOfficeCode, string strContractCode, string strProjectCode, string strInstrumentCode, string strArrayLocationCode, string strLotNo)
		/// \brief (Call stored procedure: sp_IV_GetMovingAveragePriceCondition).

		/// \fn public virtual List<Nullable<int>> GetMovingNo()
		/// \brief (Call stored procedure: sp_IV_GetMovingNo).

		/// \fn public virtual List<dtOfficeCheckingList> GetOfficeCheckingList(string pC_INV_LOC)
		/// \brief (Call stored procedure: sp_IV_GetOfficeCheckingList).

		/// \fn public virtual List<tbt_AccountInstalled> GetOldestLot(string strOfficeCode, string strLocationCode, string strInstrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetOldestLot).

		/// \fn public virtual List<doProjectInformation> GetProjectInformation(string pProjectCode)
		/// \brief (Call stored procedure: Sp_IV_GetProjectInformation).

		/// \fn public virtual List<tbt_InventoryProjectWIP> GetProjectWIPInstrument(string strProjectCode)
		/// \brief (Call stored procedure: sp_IV_GetProjectWIPInstrument).

		/// \fn public virtual List<doPurchaseOrderDetail> GetPurchaseOrderDetailForMaintain(string c_CURRENCY_TYPE, string c_PURCHASE_ORDER_STATUS, string c_TRANSPORT_TYPE, string strPurchaseOrder)
		/// \brief (Call stored procedure: sp_IV_GetPurchaseOrderDetailForMaintain).

		/// \fn public virtual List<doPurchaseOrderDetail> GetPurchaseOrderDetailForRegisterStockIn(string strPurchaseOrderNo, string c_CURRENCY_TYPE, string c_PURCHASE_ORDER_STATUS, string c_TRANSPORT_TYPE)
		/// \brief (Call stored procedure: sp_IV_GetPurchaseOrderDetailForRegisterStockIn).

		/// \fn public virtual List<doPurchaseOrder> GetPurchaserOrderForMaintain(string c_PURCHASE_ORDER_STATUS, string c_TRANSPORT_TYPE, string purchaseOrderNo, string purchaseOrderStatus, string supplierCode, string transportType, string supplierName)
		/// \brief (Call stored procedure: sp_IV_GetPurchaseOrderForMaintain).

		/// \fn public virtual List<doResultGetReturnSlip> GetReturnedSlip(string strInstallationSlipNo, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE, string c_SLIP_STATUS_WAIT_FOR_RETURN)
		/// \brief (Call stored procedure: sp_IV_GetReturnedSlip).

		/// \fn public virtual List<doResultReturnInstrument> GetReturnInstrumentByInstallationSlip(string strInstallationSlipNo, string c_INV_RETURNED, string c_INV_LOC_ELIMINATION, string c_INV_SLIP_STATUS_TRANSFER, string c_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetReturnInstrumentByInstallationSlip).

		/// \fn public virtual List<doSaleInstrument> GetSaleInstrument(string strContractCode, string c_INV_AREA_NEW_SALE, string c_INV_AREA_NEW_SAMPLE)
		/// \brief (Call stored procedure: sp_IV_GetSaleInstrument).

		/// \fn public virtual List<doShelfCurrentData> GetShelfCurrentData(string officeCode, string locationCode, string shelfNo)
		/// \brief (Call stored procedure: sp_IV_GetShelfCurrentData).

		/// \fn public virtual List<doShelfCurrentData> GetShelfForChecking(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetShelfForChecking).

		/// \fn public virtual List<string> GetShelfOfArea(string areaCode, string instrumentCode, string c_INV_SHELF_TYPE_NORMAL)
		/// \brief (Call stored procedure: sp_IV_GetShelfOfArea).

		/// \fn public virtual List<dtStockCheckingList> GetStockCheckingList(string pAreaCode, string pCheckingYearMonth, string pInstrumentCode, string pInstrumentName, string pLocationCode, string pOfficeCode, string pShelfNoFrom, string pShelfNoTo, string pC_INV_AREA, string pC_INV_LOC)
		/// \brief (Call stored procedure: sp_IV_GetStockCheckingList).

		/// \fn public virtual List<dtStockOutByInstallationSlipResult> GetStockOutByInstallationSlip(string pC_INV_AREA, string pInstallSlipNo)
		/// \brief (Call stored procedure: Sp_IV_GetStockOutByInstallationSlip).

		/// \fn public virtual List<doResultGetSumPartialStockOutList> GetSumPartialStockOutList(string pC_INV_SLIP_STATUS_PARTIAL, string pContractCode)
		/// \brief (Call stored procedure: Sp_IV_GetSumPartialStockOutList).

		/// \fn public virtual List<tbs_InventoryRunningSlipNo> GetTbs_InventorySlipRunningNo(string month, string year, string officeCode, string slipid)
		/// \brief (Call stored procedure: sp_IV_GetTbs_InventorySlipRunningNo).

		/// \fn public virtual List<tbs_PurchaseOrderRunningNo> GetTbs_PurchaseOrderRunningNo(string yearCode, string monthCode, string nationCodeCode)
		/// \brief (Call stored procedure: sp_IV_GetTbs_PurchaseOrderRunningNo).

		/// \fn public virtual List<tbt_AccountInprocess> GetTbt_AccountInProcess(string locationCode, string contractCode, string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetTbt_AccountInProcess).

		/// \fn public virtual List<tbt_AccountInstalled> GetTbt_AccountInstalled(string officeCode, string locationCode, string instrumentCode, string lotNo)
		/// \brief (Call stored procedure: sp_IV_GetTbt_AccountInstalled).

		/// \fn public virtual List<tbt_AccountInstock> GetTbt_AccountInStock(string instrumentCode, string locationCode, string officecode)
		/// \brief (Call stored procedure: sp_IV_GetTbt_AccountInStock).

		/// \fn public virtual List<tbt_AccountSampleInprocess> GetTbt_AccountSampleInProcess(string locationCode, string contractCode, string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_getTbt_AccountSampleInProcess).

		/// \fn public virtual List<tbt_AccountSampleInstock> GetTbt_AccountSampleInStock(string instrumentCode, string locationCode, string officeCode)
		/// \brief (Call stored procedure: sp_IV_GetTbt_AccountSampleInStock).

		/// \fn public virtual List<tbt_InventoryBooking> GetTbt_InventoryBooking(string pContractCode)
		/// \brief (Call stored procedure: Sp_IV_GetTbt_InventoryBooking).

		/// \fn public virtual List<tbt_InventoryBookingDetail> GetTbt_InventoryBookingDetail(string pContractCode, string pInstrumentCode)
		/// \brief (Call stored procedure: Sp_IV_GetTbt_InventoryBookingDetail).

		/// \fn public virtual List<tbt_InventoryCheckingSchedule> GetTbt_InventoryCheckingSchedule(string strYearMonth)
		/// \brief (Call stored procedure: sp_IV_GetTbt_InventoryCheckingSchedule).

		/// \fn public virtual List<tbt_InventoryCheckingSlip> GetTbt_InventoryCheckingSlip(string pSlipNo, string pCheckingYearMonth, string pLocationCode, string pOfficeCode)
		/// \brief (Call stored procedure: Sp_IV_GetTbt_InventoryCheckingSlip).

		/// \fn public virtual List<tbt_InventoryCheckingTemp> GetTbt_InventoryCheckingTemp(string pCheckingYearMonth, string pLocationCode, string pOfficeCode)
		/// \brief (Call stored procedure: Sp_IV_GetTbt_InventoryCheckingTemp).

		/// \fn public virtual List<tbt_InventoryCurrent> GetTbt_InventoryCurrent(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetTbt_InventoryCurrent).

		/// \fn public virtual List<tbt_InventoryProjectWIP> GetTbt_InventoryProjectWIP(string pProjectCode, string pAreaCode, string pInstrumentCode)
		/// \brief (Call stored procedure: Sp_IV_GetTbt_InventoryProjectWIP).

		/// \fn public virtual List<tbt_InventorySlip> GetTbt_InventorySlip(string slipNo, string installationSlipNo)
		/// \brief (Call stored procedure: sp_IV_GetTbt_InventorySlip).

		/// \fn public virtual List<tbt_InventorySlipDetail> GetTbt_InventorySlipDetail(string slipNo, Nullable<int> runningNo)
		/// \brief (Call stored procedure: sp_IV_GetTbt_InventorySlipDetail).

		/// \fn public virtual List<doTbt_InventorySlipDetailForView> GetTbt_InventorySlipDetailForView(string strInventorySlipNo, string c_INV_AREA)
		/// \brief (Call stored procedure: sp_IV_GetTbt_InventorySlipDetailForView).

		/// \fn public virtual List<tbt_InventorySlip> GetTbt_InventorySlipForReceiveReturn(string strInstallationSlipNo, string c_INV_LOC_RETURNED, string c_INV_SLIP_STATUS_COMPLETE)
		/// \brief (Call stored procedure: sp_IV_GetTbt_InventorySlipForReceiveReturn).

		/// \fn public virtual List<tbt_PurchaseOrder> GetTbt_PurchaseOrder(string purchaseOrderNo)
		/// \brief (Call stored procedure: sp_IV_GetTbt_PurchaseOrder).

		/// \fn public virtual List<tbt_PurchaseOrderDetail> GetTbt_PurchaseOrderDetail(string purchaseOrderNo, string instrumentCode)
		/// \brief (Call stored procedure: sp_IV_GetTbt_PurchaseOrderDetail).

		/// \fn public virtual List<tbt_AccountStockMoving> InsertAccountStockMoving(string xmlTbt_AccountStockMoving)
		/// \brief (Call stored procedure: sp_IV_InsertAccountStockMoving).

		/// \fn public virtual List<tbs_InventoryRunningSlipNo> InsertTbs_InventorySlipRunningNo(string runningNo, string month, string year, string officeCode, string slipid)
		/// \brief (Call stored procedure: sp_IV_InsertTbs_InventorySlipRunningNo).

		/// \fn public virtual int InsertTbs_LotRunningNo(string instrumentCode, string depreciationPeriodForContract, string startYearMonth, string lotRunningNo)
		/// \brief (Call stored procedure: sp_IV_InsertTbs_LotRunningNo).

		/// \fn public virtual List<tbs_PurchaseOrderRunningNo> InsertTbs_PurchaseOrderRunningNo(string yearCode, string monthCode, string nationCodeCode, string runningNo)
		/// \brief (Call stored procedure: sp_IV_InsertTbs_PurchaseOrderRunningNo).

		/// \fn public virtual List<tbt_AccountInprocess> InsertTbt_AccountInProcess(string xmlTbt_AccountInProcess)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_AccountInProcess).

		/// \fn public virtual List<tbt_AccountInstalled> InsertTbt_AccountInstalled(string xmltbt_AccountInstalled)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_AccountInstalled).

		/// \fn public virtual List<tbt_AccountInstock> InsertTbt_AccountInStock(string xmlTbt_AccountInStock)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_AccountInStock).

		/// \fn public virtual List<tbt_AccountSampleInprocess> InsertTbt_AccountSampleInProcess(string xmlTbt_AccountSampleInProcess)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_AccountSampleInProcess).

		/// \fn public virtual List<tbt_AccountSampleInstock> InsertTbt_AccountSampleInStock(string xmlTbt_AccountSampleInStock)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_AccountSampleInStock).

		/// \fn public virtual List<tbt_AccountStockMoving> InsertTbt_AccountStockMoving(string xmltbt_AccountStockMoving)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_AccountStockMoving).

		/// \fn public virtual List<tbt_InventoryBooking> InsertTbt_InventoryBooking(string xml_doTbt_InventoryBookings)
		/// \brief (Call stored procedure: Sp_IV_InsertTbt_InventoryBooking).

		/// \fn public virtual List<tbt_InventoryBookingDetail> InsertTbt_InventoryBookingDetail(string xml_doTbt_InventoryBookingDetails)
		/// \brief (Call stored procedure: Sp_IV_InsertTbt_InventoryBookingDetail).

		/// \fn public virtual List<tbt_InventoryCheckingSchedule> InsertTbt_InventoryCheckingSchedule(string xml)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_InventoryCheckingSchedule).

		/// \fn public virtual List<tbt_InventoryCheckingSlip> InsertTbt_InventoryCheckingSlip(string xml_doTbt_InventoryCheckingSlips)
		/// \brief (Call stored procedure: Sp_IV_InsertTbt_InventoryCheckingSlip).

		/// \fn public virtual List<tbt_InventoryCheckingSlipDetail> InsertTbt_InventoryCheckingSlipDetail(string xml_doTbt_InventoryCheckingSlipDetails)
		/// \brief (Call stored procedure: Sp_IV_InsertTbt_InventoryCheckingSlipDetail).

		/// \fn public virtual List<tbt_InventoryCurrent> InsertTbt_InventoryCurrent(string xmlTbt_InventoryCurrent)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_InventoryCurrent).

		/// \fn public virtual int InsertTbt_InventoryDepreciation(string lotNo, string instrumentCode, Nullable<decimal> accquisitionCost, string startDepreciationYearMonth, Nullable<int> totalNumDepreciation, Nullable<int> lastNumDepreciation, string lastDepreciationYearMonth, Nullable<decimal> accumulateDepreciationAmount, Nullable<decimal> monthlyDepreciationAmount, Nullable<decimal> lastDepreciationAmount, Nullable<int> totalNumDepreciationRevenue, Nullable<int> lastNumDepreciationRevenue, string lastDepreciationYearMonthRevenue, Nullable<decimal> accumulateDepreciationAmountRevenue, Nullable<decimal> monthlyDepreciationAmountRevenue, Nullable<decimal> lastDepreciationAmountRevenue, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_InventoryDepreciation).

		/// \fn public virtual List<tbt_InventoryProjectWIP> InsertTbt_InventoryProjectWIP(string xml_doTbt_InventoryProjectWIPs)
		/// \brief (Call stored procedure: Sp_IV_InsertTbt_InventoryProjectWIP).

		/// \fn public virtual List<tbt_InventorySlip> InsertTbt_InventorySlip(string xmlTbt_InventorySlilp)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_InventorySlip).

		/// \fn public virtual List<tbt_InventorySlipDetail> InsertTbt_InventorySlipDetail(string xmlTbt_InventorySlipDetail)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_InventorySlipDetail).

		/// \fn public virtual List<tbt_PurchaseOrder> InsertTbt_PurchaseOrder(string xml_Tbt_PurchaseOrder)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_PurchaseOrder).

		/// \fn public virtual List<tbt_PurchaseOrderDetail> InsertTbt_PurchaseOrderDetail(string xml_Tbt_PurchaseOrderDetail)
		/// \brief (Call stored procedure: sp_IV_InsertTbt_PurchaseOrderDetail).

		/// \fn public virtual List<doIsEmptyShelfResult> IsEmptyShelf(string strShelfNo)
		/// \brief (Call stored procedure: Sp_IV_IsEmptyShelf).

		/// \fn public virtual List<doCSVOtherFinancialAcc> OtherFinancialAcc(string pC_INV_TRANSFERTYPE_STOCKIN_PURCHASE, string pC_INV_TRANSFERTYPE_STOCKIN_SPECIAL, string pC_INV_TRANSFERTYPE_ELIMINATION, string pC_INV_TRANSFERTYPE_STOCKOUT_SPECIAL, string pC_INV_TRANSFERTYPE_FIX_ADJUSTMENT)
		/// \brief (Call stored procedure: sp_IV_OtherFinancialAcc).

		/// \fn public virtual List<dtSearchInstallationSlipResult> SearchInstallationSlip(string pC_CONFIG_WILDCARD, string pC_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03, string pC_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20, string pC_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29, string pInstallationSlipNo, Nullable<System.DateTime> pExpectedStockOutDateFrom, Nullable<System.DateTime> pExpectedStockOutDateTo, string pContractCode, string pProjectCode, string pOperationOfficeCode, string pSubContractorName)
		/// \brief (Call stored procedure: Sp_IV_SearchInstallationSlip).

		/// \fn public virtual List<dtSearchInstrumentListResult> SearchInventoryInstrumentList(string c_INV_AREA, string officeCode, string locationCode, string areaCodeStr, string shelfType, string endShelfNo, string instrumentName, string instrumentCode, string startShelfNo, string c_INV_AREA_SHORT, string c_INV_SHELF_NO_NOT_MOVE_SHELF, string c_INV_SHELF_NO_NOT_PRICE)
		/// \brief (Call stored procedure: Sp_IV_SearchInventoryInstrumentList).

		/// \fn public virtual List<dtSearchInstrumentListResult> SearchInventoryInstrumentListAllShelf(string officeCode, string locationCode, string areaCode, string startShelfNo, string endShelfNo, string instrumentCode, string instrumentName, string c_INV_AREA, string c_CONFIG_WILDCARD, string c_INV_SHELF_NO_NOT_PRICE)
		/// \brief (Call stored procedure: sp_IV_SearchInventoryInstrumentListAllShelf).

		/// \fn public virtual List<dtSearchInstrumentListResult> SearchInventoryInstrumentListIVS190(string c_INV_AREA, string c_INV_AREA_SHORT, string officeCode, string locationCode, string areaCodeStr, string shelfType, string startShelfNo, string endShelfNo, string instrumentName, string instrumentCode, string sH_NO_NOT_MOVE_SH, string sH_NO_NOT_PRICE, string transferType, string c_CONFIG_WILDCARD)
		/// \brief (Call stored procedure: Sp_IV_SearchInventoryInstrumentListIVS190).

		/// \fn public virtual List<doInventorySlip> SearchInventorySlip(string c_INV_LOC, string inventorySlipNo)
		/// \brief (Call stored procedure: sp_IV_SearchInventorySlip).

		/// \fn public virtual List<doInventorySlipDetail> SearchInventorySlipDetail(string c_INV_AREA, string inventorySlipNo)
		/// \brief (Call stored procedure: sp_IV_SearchInventorySlipDetail).

		/// \fn public virtual int UpdateCalculateDepreciation(Nullable<System.DateTime> pProcessDate, string pEmpNo)
		/// \brief (Call stored procedure: Sp_IV_UpdateCalculateDepreciation).

		/// \fn public virtual int UpdateMovingAveragePriceForInstalled(Nullable<decimal> decMovingAveragePrice, string strInstrumentCode, string strLotNo, string c_CONFIG_SCRAP_VALUE, Nullable<System.DateTime> updateDate, string updateBy, string pGUID, string pScreenID)
		/// \brief (Call stored procedure: sp_IV_UpdateMovingAveragePriceForInstalled).

		/// \fn public virtual List<tbt_AccountInstock> UpdateMovingAveragePriceForInStockGroup(Nullable<decimal> decMovingAveragePrice, string c_INV_LOC_INSTOCK, string c_INV_LOC_TRANSFER, string strInstrumentCode)
		/// \brief (Call stored procedure: sp_IV_UpdateMovingAveragePriceForInStockGroup).

		/// \fn public virtual List<tbt_AccountInstock> UpdateMovingAveragePriceForRepairingGroup(string c_INV_LOC_REPAIR_REQUEST, string c_INV_LOC_REPAIRING, string c_INV_LOC_REPAIR_RETURN, Nullable<decimal> decMovingAveragePrice, string strOfficeCode, string strInstrumentCode)
		/// \brief (Call stored procedure: sp_IV_UpdateMovingAveragePriceForRepairingGroup).

		/// \fn public virtual List<tbt_AccountInprocess> UpdateMovingAveragePriceForWIPGroup(string strProjectCode, Nullable<decimal> decMovingAveragePrice, string strInstrumentCode, string strContractCode, string c_INV_LOC_PARTIAL, string c_INV_LOC_PROJECT_WIP, string c_INV_LOC_WIP, string c_INV_LOC_UNOPERATED_WIP)
		/// \brief (Call stored procedure: sp_IV_UpdateMovingAveragePriceForWIPGroup).

        /// \fn public virtual List<tbt_InventorySlip> UpdatePartialToCompleteStatus(string pC_INV_SLIP_STATUS_COMPLETE, string pC_INV_SLIP_STATUS_PARTIAL, string pContractCode)
        /// \brief (Call stored procedure: Sp_IV_UpdatePartialToCompleteStatus).

		/// \fn public virtual List<tbs_InventoryRunningSlipNo> UpdateTbs_InventorySlipRunningNo(string month, string year, string officeCode, string slipid, string runningNo)
		/// \brief (Call stored procedure: sp_IV_UpdateTbs_InventorySlipRunningNo).

		/// \fn public virtual List<tbs_PurchaseOrderRunningNo> UpdateTbs_PurchaseOrderRunningNo(string yearCode, string monthCode, string nationCodeCode, string strRunningNo)
		/// \brief (Call stored procedure: sp_IV_UpdateTbs_PurchaseOrderRunningNo).

		/// \fn public virtual List<tbt_AccountInprocess> UpdateTbt_AccountInProcess(string xmlTbt_AccountInProcess)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_AccountInProcess).

		/// \fn public virtual List<tbt_AccountInstalled> UpdateTbt_AccountInstalled(string xmltbt_AccountInstalled)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_AccountInstalled).

		/// \fn public virtual List<tbt_AccountInstock> UpdateTbt_AccountInStock(string xmlTbt_AccountInStock)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_AccountInStock).

		/// \fn public virtual List<tbt_AccountSampleInprocess> UpdateTbt_AccountSampleInProcess(string xmlTbt_AccountSampleInProcess)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_AccountSampleInProcess).

		/// \fn public virtual List<tbt_AccountSampleInstock> UpdateTbt_AccountSampleInStock(string xmlTbt_AccountSampleInStock)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_AccountSampleInStock).

		/// \fn public virtual List<tbt_InventoryBooking> UpdateTbt_InventoryBooking(string xml_doTbt_InventoryBookings)
		/// \brief (Call stored procedure: Sp_IV_UpdateTbt_InventoryBooking).

		/// \fn public virtual List<tbt_InventoryBookingDetail> UpdateTbt_InventoryBookingDetail(string xml_doTbt_InventoryBookingDetails)
		/// \brief (Call stored procedure: Sp_IV_UpdateTbt_InventoryBookingDetail).

		/// \fn public virtual List<tbt_InventoryCheckingSchedule> UpdateTbt_InventoryCheckingSchedule(string xml)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_InventoryCheckingSchedule).

		/// \fn public virtual List<tbt_InventoryCurrent> UpdateTbt_InventoryCurrent(string xmlTbt_InventoryCurrent)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_InventoryCurrent).

		/// \fn public virtual int UpdateTbt_InventoryCurrentByLocation(string sourceOfficeCode, string destinationOfficeCode, string sourceLocationCode, string destinationLocationCode, string sourceAreaCode, string destinationAreaCode, string sourceShelfNo, string destinationShelfNo, string instrumentCode, Nullable<int> transferQty, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_InventoryCurrent_By_Location).

		/// \fn public virtual List<tbt_InventoryProjectWIP> UpdateTbt_InventoryProjectWIP(string projectCode, string areaCode, string instrumentCode, Nullable<int> addToInstrumentQty)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_InventoryProjectWIP).

		/// \fn public virtual List<tbt_InventorySlip> UpdateTbt_InventorySlip(string xmlTbt_InventorySlip)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_InventorySlip).

		/// \fn public virtual List<tbt_InventorySlipDetail> UpdateTbt_InventorySlipDetail(string xmlTbt_InventorySlipDetail)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_InventorySlipDetail).

		/// \fn public virtual List<tbt_PurchaseOrder> UpdateTbt_PurchaseOrder(string xmlPurchaseOrder)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_PurchaseOrder).

		/// \fn public virtual List<tbt_PurchaseOrderDetail> UpdateTbt_PurchaseOrderDetail(string xmlPurchasOrderDetail)
		/// \brief (Call stored procedure: sp_IV_UpdateTbt_PurchaseOrderDetail).

		/// \fn public virtual List<string> ValidateFreezeInstrumentIVP050()
		/// \brief (Call stored procedure: Sp_IV_ValidateFreezeInstrumentIVP050).


	}
}

