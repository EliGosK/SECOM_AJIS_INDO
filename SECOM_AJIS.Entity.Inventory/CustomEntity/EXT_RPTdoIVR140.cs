using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class RPTdoIVR140 {

        public string BalanceQtyShow
        {
            get
            {
                return BalanceQty == 0 ? "-" : BalanceQty.ToString("#,##0");
            }
        }

        public string PurchaseStockInQtyShow
        {
            get
            {
                return PurchaseStockInQty == 0 ? "-" : PurchaseStockInQty.ToString("#,##0");
            }
        }

        public string SpecialStockInQtyShow
        {
            get
            {
                return SpecialStockInQty == 0 ? "-" : SpecialStockInQty.ToString("#,##0");
            } 
        }

        public string ReceiveFromOfficeQtyShow
        {
            get
            {
                return ReceiveFromOfficeQty == 0 ? "-" : ReceiveFromOfficeQty.ToString("#,##0");
            }
        }

        public string ReceiveFromEliminationQtyShow
        {
            get
            {
                return ReceiveFromEliminationQty == 0 ? "-" : ReceiveFromEliminationQty.ToString("#,##0");
            }
        }

        public string AdjustStockPlusQtyShow
        {
            get
            {
                return AdjustStockPlusQty == 0 ? "-" : AdjustStockPlusQty.ToString("#,##0");
            }
        }

        public string ReturnFromWIPQtyShow
        {
            get
            {
                return ReturnFromWIPQty == 0 ? "-" : ReturnFromWIPQty.ToString("#,##0");
            }
        }

        public string ReturnFromWatingReturnQtyShow
        {
            get
            {
                return ReturnFromWatingReturnQty == 0 ? "-" : ReturnFromWatingReturnQty.ToString("#,##0");
            }
        }

        public string ReceiveRepairedQtyShow
        {
            get
            {
                return ReceiveRepairedQty == 0 ? "-" : ReceiveRepairedQty.ToString("#,##0");
            }
        }

        public string SumTotalInQtyShow
        {
            get
            {
                return SumTotalInQty == 0 ? "-" : SumTotalInQty.ToString("#,##0");
            }
        }

        public string NewStockOutQtyShow
        {
            get
            {
                return NewStockOutQty == 0 ? "-" : NewStockOutQty.ToString("#,##0");
            }
        }

        public string ProjectStockOutQtyShow
        {
            get
            {
                return ProjectStockOutQty == 0 ? "-" : ProjectStockOutQty.ToString("#,##0");
            }
        }

        public string PartialStockOutQtyShow
        {
            get
            {
                return PartialStockOutQty == 0 ? "-" : PartialStockOutQty.ToString("#,##0");
            }
        }

        public string ChangeInstallationQtyShow
        {
            get
            {
                return ChangeInstallationQty == 0 ? "-" : ChangeInstallationQty.ToString("#,##0");
            }
        }

        public string TransferToOfficeQtyShow
        {
            get
            {
                return TransferToOfficeQty == 0 ? "-" : TransferToOfficeQty.ToString("#,##0");
            }
        }

        public string TransferToRepairingQtyShow
        {
            get
            {
                return TransferToRepairingQty == 0 ? "-" : TransferToRepairingQty.ToString("#,##0");
            }
        }

        public string TransferToPreEliminationQtyShow
        {
            get
            {
                return TransferToPreEliminationQty == 0 ? "-" : TransferToPreEliminationQty.ToString("#,##0");
            }
        }

        public string AdjustStockMinusQtyShow
        {
            get
            {
                return AdjustStockMinusQty == 0 ? "-" : AdjustStockMinusQty.ToString("#,##0");
            }
        }

        public string SpacialOutQtyShow
        {
            get
            {
                return SpacialOutQty == 0 ? "-" : SpacialOutQty.ToString("#,##0");
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