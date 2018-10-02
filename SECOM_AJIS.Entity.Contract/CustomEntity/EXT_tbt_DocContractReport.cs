using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_DocContractReport_MetaData))]
    public partial class tbt_DocContractReport
    {
        public string DocumentLanguageName { get; set; }
        public string DocumentLanguageCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.DocumentLanguage, this.DocumentLanguageName);
            }
        }

    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_DocContractReport_MetaData
    {
        [DocLanguageMappingAttribute("DocumentLanguageName")]
        public string DocumentLanguage { get; set; }
    }
}
