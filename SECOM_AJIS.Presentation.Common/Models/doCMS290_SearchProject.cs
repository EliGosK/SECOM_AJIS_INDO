using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Presentation.Common
{
    /// <summary>
    /// DO for store search project of screen CMS290
    /// </summary>
    [Serializable]
    public class doCMS290_SearchProject
    {
        public string ProjectCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectName { get; set; }
        public string ProRepAdd { get; set; }
        public string PJPurName { get; set; }
        public string PJOwe1 { get; set; }
        public string PJManComName { get; set; }
        public string OthPJRelPerName { get; set; }
        public string SystemProduct { get; set; }
        public string HeadSalemaneName { get; set; }
        public string PJMangerName { get; set; }
        
    }
}
