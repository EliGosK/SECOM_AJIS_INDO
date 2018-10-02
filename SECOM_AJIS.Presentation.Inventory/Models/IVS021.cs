using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// DO for initial screen.
    /// </summary>
    public class IVS021_ScreenParameter : ScreenParameter
    {
        public List<doResultInstallationSlipForStockOut> InstallationSlipForStockOutList { get; set; }
    }

    /// <summary>
    /// DO for register installation stock-out.
    /// </summary>
    [MetadataType(typeof(IVS021_RegisterInstallationForStockOut_Param_Meta))]
    public class IVS021_RegisterInstallationForStockOut_Param
    {
        /// <summary>
        /// DO for register installation stokc-out (control ID).
        /// </summary>
        public class doDetailParameter : doResultInstallationDetailForStockOut
        {
            public string NewInstSaleCtrlID { get; set; }
            public string NewInstSampleCtrlID { get; set; }
            public string NewInstRentalCtrlID { get; set; }
            public string SecondhandInstRentalCtrlID { get; set; }
            public string RemarkCtrlID { get; set; }
        }

        public doResultInstallationSlipForStockOut header { get; set; }
        public List<IVS021_RegisterInstallationForStockOut_Param.doDetailParameter> details { get; set; }
        public string Memo { get; set; }
        public DateTime? StockOutDate { get; set; }
    }

    public class IVS021_RegisterInstallationForStockOut_Param_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                Screen = "IVS021",
                Parameter = "lblStockOutDate",
                ControlName = "txtStockOutDate")]
        public DateTime? StockOutDate { get; set; }
    }

    /// <summary>
    /// Helper class for IVS021.
    /// </summary>
    public static class IVS021_Helper
    {
        /// <summary>
        /// Convert DO:IVS021_RegisterInstallationForStockOut_Param.doDetailParameter to DO:tbt_InventorySlipDetail
        /// </summary>
        /// <param name="lst">List of IVS021_RegisterInstallationForStockOut_Param.doDetailParameter</param>
        /// <returns>List of tbt_InventorySlipDetail</returns>
        public static List<tbt_InventorySlipDetail> ToTbt_InventorySlipDetail(this List<IVS021_RegisterInstallationForStockOut_Param.doDetailParameter> lst)
        {
            List<tbt_InventorySlipDetail> lstResult = new List<tbt_InventorySlipDetail>();

            int iInvSlpDtlRunningNo = 0;

            lstResult.AddRange(
                from d in lst
                where d.NewInstSale != 0
                group d by new { InstrumentCode = d.ChildInstrumentCode, ShelfNo = d.SaleShelfNo, d.Remark } into grpShelfNo
                select new tbt_InventorySlipDetail()
                {
                    RunningNo = ++iInvSlpDtlRunningNo,
                    InstrumentCode = grpShelfNo.Key.InstrumentCode,
                    DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                    SourceAreaCode = InstrumentArea.C_INV_AREA_NEW_SALE,
                    DestinationAreaCode = InstrumentArea.C_INV_AREA_NEW_SALE,
                    SourceShelfNo = grpShelfNo.Key.ShelfNo,
                    TransferQty = grpShelfNo.Sum(d2 => d2.NewInstSale),
                    Remark = grpShelfNo.Key.Remark
                }
            );

            lstResult.AddRange(
                from d in lst
                where d.NewInstSample != 0
                group d by new { InstrumentCode = d.ChildInstrumentCode, ShelfNo = d.SampleShelfNo, d.Remark } into grpShelfNo
                select new tbt_InventorySlipDetail()
                {
                    RunningNo = ++iInvSlpDtlRunningNo,
                    InstrumentCode = grpShelfNo.Key.InstrumentCode,
                    DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                    SourceAreaCode = InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                    DestinationAreaCode = InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                    SourceShelfNo = grpShelfNo.Key.ShelfNo,
                    TransferQty = grpShelfNo.Sum(d2 => d2.NewInstSample),
                    Remark = grpShelfNo.Key.Remark
                }
            );

            lstResult.AddRange(
                from d in lst
                where d.NewInstRental != 0
                group d by new { InstrumentCode = d.ChildInstrumentCode, ShelfNo = d.RentalShelfNo, d.Remark } into grpShelfNo
                select new tbt_InventorySlipDetail()
                {
                    RunningNo = ++iInvSlpDtlRunningNo,
                    InstrumentCode = grpShelfNo.Key.InstrumentCode,
                    DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                    SourceAreaCode = InstrumentArea.C_INV_AREA_NEW_RENTAL,
                    DestinationAreaCode = InstrumentArea.C_INV_AREA_NEW_RENTAL,
                    SourceShelfNo = grpShelfNo.Key.ShelfNo,
                    TransferQty = grpShelfNo.Sum(d2 => d2.NewInstRental),
                    Remark = grpShelfNo.Key.Remark
                }
            );

            lstResult.AddRange(
                from d in lst
                where d.SecondhandInstRental != 0
                group d by new { InstrumentCode = d.ChildInstrumentCode, ShelfNo = d.SecondShelfNo, d.Remark } into grpShelfNo
                select new tbt_InventorySlipDetail()
                {
                    RunningNo = ++iInvSlpDtlRunningNo,
                    InstrumentCode = grpShelfNo.Key.InstrumentCode,
                    DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                    SourceAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL,
                    DestinationAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL,
                    SourceShelfNo = grpShelfNo.Key.ShelfNo,
                    TransferQty = grpShelfNo.Sum(d2 => d2.SecondhandInstRental),
                    Remark = grpShelfNo.Key.Remark
                }
            );

            return lstResult;
        }

        /// <summary>
        /// Convert DO:IVS021_RegisterInstallationForStockOut_Param.doDetailParameter to DO:doInstrument
        /// </summary>
        /// <param name="lst">List of IVS021_RegisterInstallationForStockOut_Param.doDetailParameter</param>
        /// <returns>List of doInstrument</returns>
        public static List<doInstrument> ToDoInstrument(this List<IVS021_RegisterInstallationForStockOut_Param.doDetailParameter> lst)
        {
            List<doInstrument> lstResult = new List<doInstrument>();

            lstResult.AddRange(
                from d in lst
                where (d.NewInstSale + d.NewInstSample + d.NewInstRental + d.SecondhandInstRental) != 0
                group d by (d.ParentInstrumentCode == null ? d.ChildInstrumentCode : d.ParentInstrumentCode) into grpParentInst
                select grpParentInst.First(d2 => true) into tmpParent
                select new doInstrument()
                {
                    InstrumentCode = (tmpParent.ParentInstrumentCode == null ? tmpParent.ChildInstrumentCode : tmpParent.ParentInstrumentCode),
                    StockOutQty = (tmpParent.NewInstSale + tmpParent.NewInstSample + tmpParent.NewInstRental + tmpParent.SecondhandInstRental)
                }
            );

            return lstResult;
        }
    }
}
