using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Master.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of table billing client
    /// </summary>
    public partial class tbm_BillingClient
    {
        public string BillingClientCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string Nationality { get; set; }
        public string BusinessTypeName { get; set; }
    }
    [MetadataType(typeof(tbm_BillingClientCondition_MetaData))]
    public class tbm_BillingClientCondition : tbm_BillingClient
    {
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    /// <summary>
    /// Do Of billing client condition meta data
    /// </summary>
    public class tbm_BillingClientCondition_MetaData
    {
        [NotNullOrEmpty]
        public string BillingClientCode { get; set; }
        [NotNullOrEmpty]
        public string CustTypeCode { get; set; }
        [NotNullOrEmpty]
        public string NameEN { get; set; }
        //[NotNullOrEmpty]
        //public string NameLC { get; set; }
        [NotNullOrEmpty]
        public string AddressEN { get; set; }
        //[NotNullOrEmpty]
        //public string AddressLC { get; set; }
    }
}
