using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class RPTdoIVR141
    {
        public string BalanceQtyShow
        {
            get
            {
                return BalanceQty == 0 ? "-" : BalanceQty.ToString("#,##0");
            }
        }

        public string RecvOfficeQtyShow
        {
            get
            {
                return RecvOfficeQty == 0 ? "-" : RecvOfficeQty.ToString("#,##0");
            }
        }

        public string SumTotalInQtyShow
        {
            get
            {
                return SumTotalInQty == 0 ? "-" : SumTotalInQty.ToString("#,##0");
            }
        }

        public string TrnfOfficeQtyShow
        {
            get
            {
                return TrnfOfficeQty == 0 ? "-" : TrnfOfficeQty.ToString("#,##0");
            }
        }

        public string NewStkOutQtyShow
        {
            get
            {
                return NewStkOutQty == 0 ? "-" : NewStkOutQty.ToString("#,##0");
            }
        }

        public string ChgeMTQtyShow
        {
            get
            {
                return ChgeMTQty == 0 ? "-" : ChgeMTQty.ToString("#,##0");
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