using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Inventory.Models.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using System.IO;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter of IVS090 screen
    /// </summary>
    public class IVS090_ScreenParameter : ScreenParameter
    {
        public string Location { get; set; }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }        
        public DateTime? StockOutDate { get; set; }

        [KeepSession]
        public doOffice office { get; set; }
        public string SlipNoReportPath { get; set; }
        public string slipNo { set; get; }
        public List<IVS090INST> ElemInstrument { get; set; }

        public bool IsSpecialOutMaterial { get; set; }
        public string[] SpecialOutMaterialAreaCode { get; set; }
        public string[] AvailableAreaCode { get; set; }
    }

    /// <summary>
    /// DO of Instrument data
    /// </summary>
    public class IVS090INST
    {
        public string Instrumentcode { get; set; }
        public string InstrumentName { get; set; }
        public string AreaCode { get; set; }
        public string AreaCodeName { get; set; }
        public string  ShelfNo { get; set; }
        public int InstrumentQty { get; set; }
        public int StockOutQty { get; set; }
        public decimal TransferAmount { get; set; }
        public string StockOutQty_view { get { return CommonUtil.TextNumeric(this.StockOutQty); } }
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string row_id { get; set; }
        public string StockOutQty_id { get; set; }
        public bool IsError { get; set; }
        public string TransferAmountCurrencyType { get; set; }

        public string TextTransferAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.TransferAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.TransferAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

    }

    /// <summary>
    /// DO of Register data
    /// </summary>
    [MetadataType(typeof(IVS090RegisterCond_Meta))]
    public class IVS090RegisterCond
    {
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string SourceLoc { get; set; }
        public List<IVS090INST> StockInInstrument { get; set; }
        public DateTime? StockOutDate { get; set; }
    }

    /// <summary>
    /// DO of Search condition
    /// </summary>
    [MetadataType(typeof(IVS090SearchCond_Meta))]
    public class IVS090SearchCond
    {
        public string SourceLoc { get; set; }
        public string InstName { get; set; }
        public string InstCode { get; set; }
        public string InstArea { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS090RegisterCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                Screen = "IVS090",
                Parameter = "lblApproveNo", 
                ControlName = "ApproveNo")]
        public string ApproveNo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                Screen = "IVS021",
                Parameter = "lblStockOutDate",
                ControlName = "txtStockOutDate")]
        public DateTime? StockOutDate { get; set; }
    }

    public class IVS090SearchCond_Meta
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string InstCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string InstName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string InstArea { get; set; }
    }

}