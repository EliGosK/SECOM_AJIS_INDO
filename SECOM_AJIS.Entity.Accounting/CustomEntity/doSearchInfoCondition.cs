using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.Accounting
{
    public class doSearchInfoCondition
    {
        //ACS010_SearchCriteria
        public DateTime? SearchTargetFrom { get; set; }
        public DateTime? SearchTargetTo { get; set; }
        public DateTime? SearchGenerateFrom { get; set; }
        public DateTime? SearchGenerateTo { get; set; }
        public string SearchDocumentNo { get; set; }
        public string SearchDocumentCode { get; set; }
        public int? SearchMonth { get; set; }
        public int? SearchYear { get; set; }


    }


    public class doGenerateInfoCondition
    {
        //ACS010_GenerateCriteria
        public string documentCode { get; set; }
        public DateTime? generateTargetFrom { get; set; }
        public DateTime generateTargetTo { get; set; }

    }
}
