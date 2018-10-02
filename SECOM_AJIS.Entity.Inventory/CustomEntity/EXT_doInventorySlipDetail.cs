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
    [MetadataType(typeof(doInventorySlipDetail_Meta))]
    public partial class doInventorySlipDetail
    {
        [LanguageMapping]
        public string SourceAreaName { get; set; }

        [LanguageMapping]
        public string DestinationAreaName { get; set; }

        public string InstrumentAreaCodeName
        {
            get
            {
                string res = "-";
                if (!CommonUtil.IsNullOrEmpty(this.DestinationAreaCode))
                {
                    res = CommonUtil.TextCodeName(this.DestinationAreaCode, this.DestinationAreaName);
                }
                return res;
            }
        }
    }

    /// <summary>
    /// DO of inventory instrument for register.
    /// </summary>
    public class doIvs012Inventory
    {
        public string InstrumentCode { get; set; }
        public double? InstrumentTotalPrice { get; set; } //public double StockInUnitPrice { get; set; } //Modify by Jutarat A. on 10042013
        public decimal? InstrumentAmountUsd { get; set; }
        public string InstrumentAmountCurrencyType { get; set; }
        public Int32 StockInQty { get; set; }
        public bool InstrumentTotalPriceEnable { get; set; } //public bool StockInUnitPriceEnable { get; set; } //Modify by Jutarat A. on 10042013
        public string InstrumentArea { get; set; }
        public string txtInstrumentTotalPrice { get; set; } //public string txtStockInUnitPrice { get; set; } //Modify by Jutarat A. on 10042013
        public Int32 RunningNo { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doInventorySlipDetail_Meta
    {
    }

}
