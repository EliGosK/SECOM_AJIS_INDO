using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;
namespace SECOM_AJIS.Presentation.Billing.Models
{
    /// <summary>
    /// Screen parameter for BLS090
    /// </summary>
    public class BLS090_ScreenParameter : ScreenParameter
    {
        public List<dtBillingBasicForRentalList> DetailData { set; get; }
        public List<BLS090_DetailData> RegisterData { set; get; }
        public decimal? TotalFee { set; get; }
        public decimal? OrderContractFee { set; get; }
    }

    /// <summary>
    /// Data object for Detail data for BLS090
    /// </summary>
    public class BLS090_DetailData : dtBillingBasicForRentalList
    {
        public string txtNewBillingFeeID { set; get; }
        public string row_id { set; get; }
        public decimal? NewBillingFee { set; get; }
    }

    
}

