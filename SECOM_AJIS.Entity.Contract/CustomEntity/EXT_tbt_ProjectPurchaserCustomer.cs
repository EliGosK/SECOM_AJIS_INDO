using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_ProjectPurchaserCustomer_MetaData))]
    public partial class tbt_ProjectPurchaserCustomer
    {
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_ProjectPurchaserCustomer_MetaData
    {

        public string CustCode { get; set; }
        [CodeNullOtherNotNull("CustCode",Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS230",
                        Parameter = "headerProjectPurchaser")]
        public string CustNameEN { get; set; }
        [CodeNullOtherNotNull("CustCode",Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS230",
                        Parameter = "headerProjectPurchaser")]
        public string CustNameLC { get; set; }
        [CodeNullOtherNotNull("CustCode",Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS230",
                        Parameter = "headerProjectPurchaser")]
        public string CustTypeCode { get; set; }
    }
}
