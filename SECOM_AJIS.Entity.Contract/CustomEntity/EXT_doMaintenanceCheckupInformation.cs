using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class doMaintenanceCheckupInformation
    {
        CommonUtil c = new CommonUtil();

        [LanguageMapping]
        public string ProductName { get; set; }

        public string CheckupNoShow
        {
            get
            {
                return (this.CheckupNo == null) ? "" : CheckupNo.ToString().PadLeft(7, '0');
            }
        }

        public string ContractCodeShort
        {
            get
            {
                return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string RealCustomerCustCodeShow
        {
            get
            {
                return c.ConvertCustCode(this.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string SiteCodeShow
        {
            get
            {
                return c.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }
}
