using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// DO for initial screen.
    /// </summary>
    public class IVS270_ScreenParameter : ScreenParameter
    {

    }

    /// <summary>
    /// DO for calculate instrument amount.
    /// </summary>
    public class IVS270_CalculateAmountParam
    {
        public string OfficeCode { get; set; }
        public string LocationCode { get; set; }
        public string AreaCode { get; set; }
        public string ShelfNo { get; set; }
        public string InstrumentCode { get; set; }
        public int? CurrentStockQty { get; set; }
        public int? StockOutQty { get; set; }
        public decimal? StockOutAmount { get; set; }
        public string StockOutQtyCtrlID { get; set; }
        public string GridRowId { get; set; }
        public int InputOrder { get; set; }
        public decimal UnitPrice { get; set; }
        public string StockOutAmountCurrencyType { get; set; }
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        public string TextTransferAmount
        {
            get
            {
                StockOutAmount = this.StockOutQty * this.UnitPrice;
                string txt = CommonUtil.TextNumeric(this.StockOutAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.StockOutAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }

    /// <summary>
    /// DO for register stock-out.
    /// </summary>
    public class IVS270_RegisterProjectStockOutParam
    {
        public bool? IsRetrievePressed { get; set; }
        public string ProjectCode { get; set; }
        public string Memo { get; set; }
        public List<IVS270_CalculateAmountParam> Details { get; set; }
    }

}
