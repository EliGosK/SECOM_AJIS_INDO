using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_DraftSaleInstrument_MetaData))]
    public partial class tbt_DraftSaleInstrument
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
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_DraftSaleInstrument_MetaData
    {
        [InstrumentMapping(new string[] { "InstrumentName", "InstrumentTypeCode", "LineUpTypeCode" })]
        public string InstrumentCode { get; set; }
        [LineUpTypeMapping("LineUpTypeName")]
        public string LineUpTypeCode { get; set; }
    }
}
