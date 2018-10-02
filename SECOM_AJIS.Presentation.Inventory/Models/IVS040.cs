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

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS040.
    /// </summary>
    public class IVS040_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doOffice office { get; set; }

        public string Location { get; set; }
        public string DestinationLocation { get; set; }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public List<IVS040INST> ElemInstrument { get; set; }
        public string slipNo { get; set; }
        public string reportFilePath { get; set; }

    }

    /// <summary>
    /// DO of instrument.
    /// </summary>
    public class IVS040INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string AreaCode { get; set; }
        public string AreaCodeName { get; set; }
        public string ShelfNo { get; set; }
        public int InstrumentQty { get; set; }
        public int TransferInstrumentQty { get; set; }
        public decimal TransferAmount { get; set; }
        public string TransferAmount_view { get { return CommonUtil.TextNumeric(this.TransferAmount); } }
        public string row_id { get; set; }
        public string TransQtyID { get; set; }
        public bool IsError { get; set; }
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
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
    /// DO of confirm information.
    /// </summary>
    [MetadataType(typeof(IVS040RegisterCond_Meta))]
    public class IVS040RegisterCond
    {
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string SourceLoc { get; set; }

    }

    /// <summary>
    /// Search condition of screen IVS040.
    /// </summary>
    [MetadataType(typeof(IVS040SearchCond_Meta))]
    public class IVS040SearchCond
    {
        public string SourceLoc { get; set; }
        public string DestinationLoc { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentCode { get; set; }
        public string AreaCode { get; set; }

    }

}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS040RegisterCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                   Screen = "IVS040",
                   Parameter = "lblApproveNo",
                   ControlName = "ApproveNo")]
        public string ApproveNo { get; set; }

    }


    public class IVS040SearchCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS040",
                     Parameter = "lblSourceLocation",
                     ControlName = "SourceLocation")]
        public string SourceLoc { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS040",
                     Parameter = "lblDestinationLocation",
                     ControlName = "DestinationLocation")]
        public string DestinationLoc { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string InstrumentName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string InstrumentCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string AreaCode { get; set; }
    }

}