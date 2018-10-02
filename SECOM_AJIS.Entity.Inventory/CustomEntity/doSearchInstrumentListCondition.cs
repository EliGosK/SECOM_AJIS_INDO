using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Search condition for inventory instrument.
    /// </summary>
    public class doSearchInstrumentListCondition
    {
        public string OfficeCode { get; set; }
        public string LocationCode { get; set; }
        public string AreaCode { get; set; }
        public List<string> AreaCodeList { get; set; }
        public string ShelfType { get; set; }
        public string StartShelfNo { get; set; }
        public string EndShelfNo { get; set; }
        public string Instrumentcode { get; set; }
        public string InstrumentName { get; set; }
        public List<string> ExcludeAreaCode { get; set; }
    }



}