using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do of billing client\
    /// </summary>
    [MetadataType(typeof(dtBillingClientData_MetaData))]
    public partial class dtBillingClientData
    {
        private string _BillingClientCodeShort;
        public string BillingClientCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _BillingClientCodeShort = value; }
        }

        [LanguageMapping]
        public string CustTypeName { get; set; }

        [LanguageMapping]
        public string BusinessTypeName { get; set; }

        [LanguageMapping]
        public string Nationality { get; set; }

        [LanguageMapping]
        public string CompanyTypeName { get; set; }
    }


    public class dtBillingClientData_MetaData
    {
        [NotNullOrEmpty]
        public string BillingClientCode { get; set; }
    }
}

