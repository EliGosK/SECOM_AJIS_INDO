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

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter of IVS030 screen
    /// </summary>
    public class IVS030_ScreenParameter : ScreenParameter
    {
        public string SlipNo { get; set; }
        public SlipType SlipSelectType { get; set; }
        public bool IsError { get; set; }
        public List<IVS030INST> ElemInstrument { get; set; }
        public enum SlipType
        {
            InstallationSlip = 1,
            ProjectReturnSlip
        }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string ContractCode { get; set; }
        [KeepSession]
        public doOffice office { get; set; }
        public string ServiceTypeCode { get; set; }
        public List<tbt_InventorySlip> lstInventorySlip { get; set; } //Add by Jutarat A. on 30052013
    }

    /// <summary>
    /// DO of Instrument data
    /// </summary>
    public class IVS030INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public int? ContractRemoveQty { get; set; }
        public int? RemoveQty { get; set; }
        public int? NotInstalledQty { get; set; }
        public int? UnRemovableQty { get; set; }
        public string row_id { get; set; }
        public bool IsError { get; set; }
    }

    /// <summary>
    /// DO of Register data
    /// </summary>
    public class IVS030ConfirmCond
    {
        public string SlipNo { get; set; }
        public SlipType SlipSelectType { get; set; }
        public enum SlipType
        {
            InstallationSlip = 1,
            ProjectReturnSlip
        }
        public List<IVS030INST> SlipDetail { get; set; }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO of Search condition
    /// </summary>
    [MetadataType(typeof(IVS030SearchCond_Meta))]
    public class IVS030SearchCond
    {
        public string SlipNo { get; set; }
        public SlipType SlipSelectType { get; set; }
        public enum SlipType
        {
            InstallationSlip = 1,
            ProjectReturnSlip
        }
    }

}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS030SearchCond_Meta
    {
        [NotNullOrEmpty(Module = MessageUtil.MODULE_INVENTORY, MessageCode = MessageUtil.MessageList.MSG4051, ControlName = "InstallationReturnSlipNo")]
        public string SlipNo { get; set; }
    }
}