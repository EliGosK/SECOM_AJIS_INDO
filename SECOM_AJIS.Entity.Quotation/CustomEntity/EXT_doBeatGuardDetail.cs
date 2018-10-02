using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(doBeatGuardDetail_MetaData))]
    public partial class doBeatGuardDetail
    {
        public string NumOfDateCodeName
        {
            get
            {
                if (this.NumOfDate == null)
                    return string.Empty;
                return CommonUtil.TextCodeName(this.NumOfDate.Value.ToString(), this.NumOfDateName);
            }
        }
    }

    #region Meta Data

    public class doBeatGuardDetail_MetaData
    {
        [NumOfDateMapping("NumOfDateName")]
        public decimal? NumOfDate { get; set; }

        [LanguageMapping]
        public string NumOfDateName { get; set; }
    }

    #endregion
}
