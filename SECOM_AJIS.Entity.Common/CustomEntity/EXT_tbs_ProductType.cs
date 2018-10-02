using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class tbs_ProductType
    {
        public string ProductTypeName { get; set; }
        public string ProductTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ProductTypeCode,
                                                this.ProductTypeName);
            }
        }

        public string ProvideServiceName { get; set; }
    }
}
