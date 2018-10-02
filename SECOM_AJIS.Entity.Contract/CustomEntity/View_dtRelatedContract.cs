using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtRelatedContract : dtRelatedContract
    {
        public int EnableContractBasic { get { return (this.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL ? 1 : 0); } }
        public int EnableSaleBasic { get { return (this.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE ? 1 : 0) ; } }
        
    }
}
