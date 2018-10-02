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
    public class doGetCheckingDetailList
    {
        public string CheckingYearMonth { get; set; }
        public string OfficeCode { get; set; }
        public string LocationCode { get; set; }
        public string AreaCode { get; set; }
    }



}