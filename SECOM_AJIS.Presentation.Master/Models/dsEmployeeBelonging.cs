using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Presentation.Master.Models;
namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// DO for collect employee information.
    /// </summary>
    public class dsEmployeeBelonging
    {
        public MAS070_SaveEmployee employee { get; set; }
        public IList<View_tbm_Belonging> belongingList { get; set; }
        public List<View_tbm_Belonging> delBelList { get; set; }
    }
}
