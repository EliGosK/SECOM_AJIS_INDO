using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS190.
    /// </summary>
    public class IVS190_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public List<doOffice> HeaderOffice { set; get; }
        [KeepSession]
        public List<doMiscTypeCode> Miscellaneous { set; get; }
        public IVS190_RegisterData RegisterData { set; get; }
        public string SlipNo { set; get; }

        [KeepSession]
        public string SlipNoReportPath { set; get; }

        [KeepSession]
        public bool DisableMinus { set; get; }
    }

    /// <summary>
    /// DO for register data.
    /// </summary>
    public class IVS190_RegisterData
    {
        public IVS190_HeaderData Header { set; get; }
        public List<IVS190_DetailData> Detail { set; get; }
    }

    public class IVS190_HeaderData
    {
        public string ApproveNo { set; get; }
        public string Memo { set; get; }
        public DateTime? TransferDate { set; get; }
    }

    /// <summary>
    /// DO of instrument in Fix Adjustment Instrument table.
    /// </summary>
    public class IVS190_DetailData : dtSearchInstrumentListResult
    {
        public string txtStockAdjQtyID { set; get; }
        public string row_id { set; get; }
        public int? AccumulateSumQty { set; get; }
        public decimal? AdjustAmount { set; get; }
        public string AdjustAmountCurrencyType { set; get; }
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        public string TextTransferAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.AdjustAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.AdjustAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}
