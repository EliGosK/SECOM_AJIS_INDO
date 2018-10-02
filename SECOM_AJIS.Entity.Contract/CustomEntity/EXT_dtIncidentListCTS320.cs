using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtIncidentListCTS320
    {
        //public List<dtIncidentOccSite> IncidentOccurringSite { get; set; }

        //public List<dtIncidentOccContract> IncidentOccurringContract { get; set; }

        [LanguageMapping]
        public string ConChiefEmpFirstName { get; set; }
        [LanguageMapping]
        public string ConChiefEmpLastName { get; set; }
        [LanguageMapping]
        public string CorrEmpFirstName { get; set; }
        [LanguageMapping]
        public string CorrEmpLastName { get; set; }
        [LanguageMapping]
        public string IncidentTypeName { get; set; }
        [LanguageMapping]
        public string IncidentStatusName { get; set; }

        [LanguageMapping]
        public string ConChiefEmpName { get; set; }
        [LanguageMapping]
        public string ChiefEmpName { get; set; }
        [LanguageMapping]
        public string CorrEmpName { get; set; }
        [LanguageMapping]
        public string AsstEmpName { get; set; }

        //public string ConChiefEmpNameEN { get; set; }
        //public string ChiefEmpNameEN { get; set; }
        //public string CorrEmpNameEN { get; set; }
        //public string AsstEmpNameEN { get; set; }

        //public string ConChiefEmpNameLC { get; set; }
        //public string ChiefEmpNameLC { get; set; }
        //public string CorrEmpNameLC { get; set; }
        //public string AsstEmpNameLC { get; set; }
    }
}
