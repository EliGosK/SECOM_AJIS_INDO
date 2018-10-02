using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtARListCTS370
    {
        [LanguageMapping]
        public string CustFullName { get; set; }
        [LanguageMapping]
        public string SiteName { get; set; }
        [LanguageMapping]
        public string ContractTargetPurchaseCustFullName { get; set; }
        [LanguageMapping]
        public string QuotationTargetCustFullName { get; set; }
        [LanguageMapping]
        public string ApprEmpName { get; set; }
        [LanguageMapping]
        public string ApprEmpFirstName { get; set; }
        [LanguageMapping]
        public string ApprEmpLastName { get; set; }
        [LanguageMapping]
        public string ARTypeName { get; set; }
        [LanguageMapping]
        public string ReqEmpName { get; set; }
        [LanguageMapping]
        public string ReqEmpFirstName { get; set; }
        [LanguageMapping]
        public string ReqEmpLastName { get; set; }
        [LanguageMapping]
        public string ARStatusName { get; set; }
    }
}
