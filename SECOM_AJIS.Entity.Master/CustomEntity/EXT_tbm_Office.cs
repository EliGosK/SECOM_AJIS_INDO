using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    [MetadataType(typeof(tbm_Office_MetaData))]
    public partial class tbm_Office
    {
        public string OfficeName { get; set; }

        public string OfficeDisplay {
            get {
                return CommonUtil.TextCodeName(OfficeCode, OfficeName);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    public class tbm_Office_MetaData
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
    }
}
