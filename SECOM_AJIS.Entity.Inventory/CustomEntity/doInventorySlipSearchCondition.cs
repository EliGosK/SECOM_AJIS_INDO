using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Inventory slip search condition.
    /// </summary>
    [MetadataType(typeof(doInventorySlipSearchCondition_Meta))]
    public class doInventorySlipSearchCondition
    {
        public string SlipNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string StockInFlag { get; set; }
        public string DeliveryOrderNo { get; set; }
        public DateTime? StockInDateFrom { get; set; }
        public DateTime? StockInDateTo { get; set; }
        public string RegisterAssetFlag { get; set; }
        public string Memo { get; set; }

    }
}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doInventorySlipSearchCondition_Meta
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string SlipNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string PurchaseOrderNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string StockInFlag { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string DeliveryOrderNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public DateTime? StockInDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public DateTime? StockInDateTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string RegisterAssetFlag { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string Memo { get; set; }

    }
}