using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtCustomerListGrp
    {
        CommonUtil c = new CommonUtil();
        public string CustCodeShort
        {
            get { return c.ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        public string CustNameForShow
        {
            get { return "(1) " + this.CustNameEN + "<br>(2) " + this.CustNameLC; }
        }

        public string AddressFullForShow
        {
            get { return "(1) " + this.AddressFullEN + "<br>(2) " + this.AddressFullLC; }
        }
    }
}



