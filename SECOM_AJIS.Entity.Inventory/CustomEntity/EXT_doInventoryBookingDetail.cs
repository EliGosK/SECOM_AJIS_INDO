using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util; 

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// IVS200
    /// </summary>
    public partial class doInventoryBookingDetail
    {
        public string ContractCodeShort
        {
            get
            {
                return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        [LanguageMapping]
        public string CustName { get; set; }
        [LanguageMapping]
        public string SiteName { get; set; }
    }
}

