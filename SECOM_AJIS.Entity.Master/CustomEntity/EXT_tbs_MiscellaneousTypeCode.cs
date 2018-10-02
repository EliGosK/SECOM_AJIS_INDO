using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbs_MiscellaneousTypeCode
    {
        [LanguageMapping]
        public string ValueDisplay
        {
            get;
            set;
        }
        public string ValueCodeDisplay
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.TextCodeName(this.ValueCode, this.ValueDisplay);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
}