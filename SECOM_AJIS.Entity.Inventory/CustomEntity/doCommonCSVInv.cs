using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public class doCommonCSVInv
    {
        public List<doCSVInvDepreciationAcc> InvDepreciationAcc { get; set; }
        public List<doCSVassetAmountAcc> AssetAmountAcc { get; set; }
        public List<doCSVMovingAssetAcc> MovingAssetAcc { get; set; }
        public List<doCSVOtherFinancialAcc> OtherFinancialAcc { get; set; }
    }
}
