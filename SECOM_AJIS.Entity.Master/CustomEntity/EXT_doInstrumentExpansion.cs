using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of instrument expansion
    /// </summary>
    public partial class doInstrumentExpansion
    {
        [LanguageMapping]
        public string InstrumentNameForCustomer { get; set; }

        [LanguageMapping]
        public string LineUpTypeName { get; set; }

        //public string ToJson
        //{
        //    get { return CommonUtil.CreateJsonString(this); }
        //}
    }
}