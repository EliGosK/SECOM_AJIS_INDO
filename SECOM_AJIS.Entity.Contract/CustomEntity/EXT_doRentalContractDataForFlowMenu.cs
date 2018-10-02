using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class doRentalContractDataForFlowMenu
    {
        public string ContractCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string ContractTargetCustCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertCustCode(this.ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string RealCustomerCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertCustCode(this.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string SiteCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

    }
}
