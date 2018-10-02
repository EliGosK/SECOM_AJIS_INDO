using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_DraftRentalInstrument_MetaData))]
    public partial class tbt_DraftRentalInstrument
    {
        public string InstrumentName { get; set; }
        public string LineUpTypeCode { get; set; }
        public string LineUpTypeName { get; set; }
        public string LineUpTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.LineUpTypeCode, this.LineUpTypeName);
            }

        }
        public Nullable<bool> InstrumentFlag { get; set; }
        public Nullable<bool> ControllerFlag { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_DraftRentalInstrument_MetaData
    {
        [InstrumentMapping(new string[]{ "InstrumentName", "InstrumentTypeCode", "LineUpTypeCode", "InstrumentFlag", "ControllerFlag" })]
        public string InstrumentCode { get; set; }
        [LineUpTypeMapping("LineUpTypeName")]
        public string LineUpTypeCode { get; set; }
    }
}
