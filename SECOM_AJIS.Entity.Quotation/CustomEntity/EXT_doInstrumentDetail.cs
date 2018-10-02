using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(doInstrumentDetail_MetaData))]
	public partial class doInstrumentDetail
	{
        [LanguageMapping]
        public string LineUpTypeName { get; set; }

        public string LineUpTypeCodeName
        {
            get
            {
                return string.Format("{0}: {1}", this.LineUpTypeCode, this.LineUpTypeName);
            }

        }
        public decimal? InstalledQty
        {
            get
            {
                if (this.InstrumentQty == null)
                    return null;

                decimal total = this.InstrumentQty.Value;
                if (this.AddQty != null)
                    total += this.AddQty.Value;
                if (this.RemoveQty != null)
                    total -= this.RemoveQty.Value;

                return total;
            }
        }

        public bool? ControllerFlag { get; set; }
        public bool? InstrumentFlag { get; set; }
        public bool? SaleFlag { get; set; }
        public bool? RentalFlag { get; set; }
	}

    #region Meta Data

    public class doInstrumentDetail_MetaData
    {
        [LineUpTypeMapping("LineUpTypeName")]
        public string LineUpTypeCode{get;set;}
    }

    #endregion
}
