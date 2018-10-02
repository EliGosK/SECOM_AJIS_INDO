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
    [MetadataType(typeof(dtResultIVS220_Meta))]
    public partial class dtResultIVS220
    {
        CommonUtil com = new CommonUtil();

        public string AreaName { get; set; }

        public string ContractCodeShort
        {
            get
            {
                return com.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string InstrumentCodeAndName
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.InstrumentCode, this.InstrumentName);
            }
        }

        public string SupplierName { get; set; }

        public string SlipIdAndSlipNo 
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.SlipId, this.InventorySlipNo);
            }
        }

        public string Costs 
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}"
                    , (this.UnitPrice == null ? "-" : this.UnitPrice.Value.ToString("#,##0.00"))
                    , (this.TotalCost == null ? "-" : this.TotalCost.Value.ToString("#,##0.00"))
                );
            }
        }

        public string CustFullName { get; set; }

        public string SupplierNameAndPurchaseNo
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}", this.SupplierName ?? "-", this.PurchaseOrderNo ?? "-", this.Memo ?? "-");
            }
        }
    }

}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtResultIVS220_Meta
    {
        [LanguageMapping]
        public string AreaName { get; set; }

        [GridToolTip("AreaName")]
        public string AreaNameShort { get; set; }

        [LanguageMapping]
        public string SupplierName { get; set; }

        [LanguageMapping]
        public string CustFullName { get; set; }

    }

}
