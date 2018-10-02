using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(doSentryGuardDetail_MetaData))]
    public partial class doSentryGuardDetail
    {
        public string ToJson
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.CreateJsonString(this);
            }
        }
        public string CostPerHourCurrencyType { get; set; }

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferCostPerHour
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.CostPerHour);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.CostPerHourCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class doSentryGuardDetail_MetaData
    {
        [LanguageMapping]
        public string SentryGuardTypeName { get; set; }

        [SentryGuardTypeMapping("SentryGuardTypeName")]
        public string SentryGuardTypeCode { get; set; }
    }
}