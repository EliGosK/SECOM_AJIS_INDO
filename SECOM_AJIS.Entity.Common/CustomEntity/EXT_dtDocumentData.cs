using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

using SECOM_AJIS.DataEntity.Common.MetaData;

namespace SECOM_AJIS.DataEntity.Common
{
    [MetadataType(typeof(dtDocumentData_MetaData))]
    public partial class dtDocumentData
    {
        public string LocationName { get; set; }
        public string LocationCodeName { get { return CommonUtil.TextCodeName(this.LocationCode ,this.LocationName); } }
        public string MonthYear
        {
            get
            {
                string strMonth = "";
                if (this.Month.HasValue)
                {
                    var date = new DateTime(2000,this.Month.Value,1);
                    strMonth = date.ToString("MMMM");
                }
            return string.Format("{0}/{1}", strMonth, this.Year);
        }
        }

        public DateTime? GenerateDateFrom { get; set; }
        public DateTime? GenerateDateTo { get; set; }
    }


}

namespace SECOM_AJIS.DataEntity.Common.MetaData
{
    public class dtDocumentData_MetaData
    {
        [InstrumentLocationMapping("LocationName")]
        public string LocationCode { get; set; }

        [LanguageMapping]
        public string ConOfficeCodeName { get; set; }
        [LanguageMapping]
        public string OperOfficeCodeName { get; set; }
        [LanguageMapping]
        public string BillOfficeCodeName { get; set; }
        [LanguageMapping]
        public string IssueOfficeCodeName { get; set; }
        [LanguageMapping]
        public string DocumentName { get; set; }

    }
}
