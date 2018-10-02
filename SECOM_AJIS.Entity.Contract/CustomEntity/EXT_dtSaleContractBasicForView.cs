using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtSaleContractBasicForView
    {
        public string ChangeTypeName { get; set; }
        public string ProductName { get; set; }
        public string SalesTypeName { get; set; }
        [LanguageMapping]
        public string SiteName { get; set; }
        [LanguageMapping]
        public string PurchaserName { get; set; }
        [LanguageMapping]
        public string SaleProcessManageStatusName { get; set; }
        [LanguageMapping]
        public string OfficeName { get; set; }
    }
}
