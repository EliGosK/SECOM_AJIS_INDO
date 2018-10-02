using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(doPurchaseOrder_Meta))]
    public partial class doPurchaseOrder
    {
        public string SupplierName { get; set; }
        public string PurchaseOrderStatusName { get; set; }

        public string TransportTypeName { get; set; }


        public DateTime? POIssueDate
        {
            get
            {
                return this.CreateDate;
            }
        }

        public string SupplierCodeName
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.SupplierCode, this.SupplierName);
            }
        }

        public string POStatusAndDeliveryDate
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.PurchaseOrderStatusName, (this.ShippingDate == null ? "" : this.ShippingDate.Value.ToString("dd-MMM-yyyy")));
            }
        }
    }

    /// <summary>
    /// DO for stored information of purchase order used as search condition.
    /// </summary>
    [MetadataType(typeof(doPurchaseOrderSearchCond_Meta))]
    public class doPurchaseOrderSearchCond : doPurchaseOrder
    {
        public DateTime? POIssueDateFrom { get; set; }
        public DateTime? POIssueDateTo { get; set; }
        public string SearchInstrumentCode { get; set; }
        public DateTime? SearchExpectedDeliveryDateFrom { get; set; }
        public DateTime? SearchExpectedDeliveryDateTo { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doPurchaseOrderSearchCond_Meta
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string PurchaseOrderNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string PurchaseOrderStatus { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string SupplierCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string TransportType { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string SupplierName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public DateTime? POIssueDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public DateTime? POIssueDateTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string SearchInstrumentCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public DateTime? SearchExpectedDeliveryDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public DateTime? SearchExpectedDeliveryDateTo { get; set; }
    }

    public class doPurchaseOrder_Meta
    {
        [LanguageMapping]
        public string SupplierName { get; set; }
        [LanguageMapping]
        [PurchaseOrderStatusMapping("PurchaseOrderStatusName")]
        public string PurchaseOrderStatusName { get; set; }
        [LanguageMapping]
        [TransportTypeMapping("TransportTypeName")]
        public string TransportTypeName { get; set; }




    }
}
