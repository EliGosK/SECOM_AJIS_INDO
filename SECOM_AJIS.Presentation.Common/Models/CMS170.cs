using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS170
    /// </summary>
    public class CMS170_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doInstrumentParam InputData { get; set; }
        [KeepSession]
        public doInstrumentSearchCondition SearchCondition { get; set; }
        [KeepSession]
        public List<doInstrumentData> ResultList { get; set; }
        [KeepSession]
        public bool DisableExpType { get; set; }
        [KeepSession]
        public bool DisableProdType { get; set; }
        [KeepSession]
        public bool DisableInstType { get; set; }
    }

    public class CMS170_SearchCondition : doInstrumentSearchCondition
    {
        public bool InstFlagMain { get; set; }
        public bool InstFlagOption { get; set; }
        public bool ExpTypeHas { get; set; }
        public bool ExpTypeNo { get; set; }
        public bool ProdTypeSale { get; set; }
        public bool ProdTypeAlarm { get; set; }
        public bool InstTypeGen { get; set; }
        public bool InstTypeMon { get; set; }
        public bool InstTypeMat { get; set; }
    }
}
