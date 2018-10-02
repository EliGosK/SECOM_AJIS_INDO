using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class RPTdoIVR143
    {
        public string BalanceQtyShow
        {
            get
            {
                return BalanceQty == 0 ? "-" : BalanceQty.ToString("#,##0");
            }
        }

        public string StrtNewServQtyShow
        {
            get
            {
                return StrtNewServQty == 0 ? "-" : StrtNewServQty.ToString("#,##0");
            }
        }

        public string ChangeInvtgPlusQtyShow
        {
            get
            {
                return ChangeInvtgPlusQty == 0 ? "-" : ChangeInvtgPlusQty.ToString("#,##0");
            }
        }

        public string CompInstQtyShow
        {
            get
            {
                return CompInstQty == 0 ? "-" : CompInstQty.ToString("#,##0");
            }
        }

        public string SumTotalInQtyShow
        {
            get
            {
                return SumTotalInQty == 0 ? "-" : SumTotalInQty.ToString("#,##0");
            }
        }

        public string CompRemvInstQtyShow
        {
            get
            {
                return CompRemvInstQty == 0 ? "-" : CompRemvInstQty.ToString("#,##0");
            }
        }

        public string ChangeInvtMinusQtyShow
        {
            get
            {
                return ChangeInvtMinusQty == 0 ? "-" : ChangeInvtMinusQty.ToString("#,##0");
            }
        }

        public string CompInstAfterQtyShow
        {
            get
            {
                return CompInstAfterQty == 0 ? "-" : CompInstAfterQty.ToString("#,##0");
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