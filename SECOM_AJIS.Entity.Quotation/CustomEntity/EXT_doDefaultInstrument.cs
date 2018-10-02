using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(doDefaultInstrument_MetaData))]
    public partial class doDefaultInstrument
    {
        public decimal InstrumentQty
        {
            get
            {
                return 0;
            }
        }
        public bool IsDefault
        {
            get
            {
                return true;
            }
        }
        public string LineUpTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.LineUpTypeCode, this.LineUpTypeName);
            }
        }
    }

    #region Meta Data

    public class doDefaultInstrument_MetaData
    {
        [LanguageMapping]
        public string LineUpTypeName { get; set; }
    }

    #endregion
}
