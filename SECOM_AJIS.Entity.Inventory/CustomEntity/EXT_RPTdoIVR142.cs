using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class RPTdoIVR142
    {
        public string BalanceQtyShow
        {
            get
            {
                return BalanceQty == 0 ? "-" : BalanceQty.ToString("#,##0");
            }
        }

        public string NewStockOutQtyShow
        {
            get
            {
                return NewStockOutQty == 0 ? "-" : NewStockOutQty.ToString("#,##0");
            }
        }

        public string ProjStockOutQtyShow
        {
            get
            {
                return ProjStockOutQty == 0 ? "-" : ProjStockOutQty.ToString("#,##0");
            }
        }

        public string PartialStkInQtyShow
        {
            get
            {
                return PartialStkInQty == 0 ? "-" : PartialStkInQty.ToString("#,##0");
            }
        }

        public string Change_MTQtyShow
        {
            get
            {
                return Change_MTQty == 0 ? "-" : Change_MTQty.ToString("#,##0");
            }
        }

        public string SumTotalInQtyShow
        {
            get
            {
                return SumTotalInQty == 0 ? "-" : SumTotalInQty.ToString("#,##0");
            }
        }

        public string CompProjQtyShow
        {
            get
            {
                return CompProjQty == 0 ? "-" : CompProjQty.ToString("#,##0");
            }
        }

        public string CusAcceptQtyShow
        {
            get
            {
                return CusAcceptQty == 0 ? "-" : CusAcceptQty.ToString("#,##0");
            }
        }

        public string CompInstQtyShow
        {
            get
            {
                return CompInstQty == 0 ? "-" : CompInstQty.ToString("#,##0");
            }
        }

        public string CancelInstQtyShow
        {
            get
            {
                return CancelInstQty == 0 ? "-" : CancelInstQty.ToString("#,##0");
            }
        }

        public string FstInstCompQtyShow
        {
            get
            {
                return FstInstCompQty == 0 ? "-" : FstInstCompQty.ToString("#,##0");
            }
        }

        public string StartNwServQtyShow
        {
            get
            {
                return StartNwServQty == 0 ? "-" : StartNwServQty.ToString("#,##0");
            }
        }

        public string CompInstAftQtyShow
        {
            get
            {
                return CompInstAftQty == 0 ? "-" : CompInstAftQty.ToString("#,##0");
            }
        }

        public string SumTotalOutQtyShow
        {
            get
            {
                return SumTotalOutQty == 0 ? "-" : SumTotalOutQty.ToString("#,##0");
            }
        }

        public string EndQtyShow
        {
            get
            {
                return EndQty == 0 ? "-" : EndQty.ToString("#,##0");
            }
        }
    }
}