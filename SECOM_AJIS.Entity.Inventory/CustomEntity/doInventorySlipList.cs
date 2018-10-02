using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(doInventorySlipList_Meta))]
    public partial class doInventorySlipList
    {
        public string RegisterAssetName { get; set; }
        public string StockInName { get; set; }
        public string StockInTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.StockInFlag, this.StockInName);
            }
        }
        public string RegisterAssetCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.RegisterAssetFlag, this.RegisterAssetName);
            }
        }

        public string SupplierName { get; set; }

        public string DeliveryOrderNoAndSupplierName
        {
            get
            {
                return string.Format(
                    "(1) {0}<br/>(2) {1}", 
                    this.DeliveryOrderNo ?? "-", 
                    this.SupplierName ?? "-"
                );
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doInventorySlipList_Meta
    {
        [LanguageMapping]
        public string StockInName { get; set; }
        [LanguageMapping]
        public string RegisterAssetName { get; set; }
        [LanguageMapping]
        public string SupplierName { get; set; }
    }

}
