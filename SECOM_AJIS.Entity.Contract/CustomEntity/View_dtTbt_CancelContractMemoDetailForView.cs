using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtTbt_CancelContractMemoDetailForView : dtTbt_CancelContractMemoDetailForView
    {
        public string Period_Extra { 

            get {
                string strStartPeriodDate = (base.StartPeriodDate != null ? base.StartPeriodDate.Value.ToString("dd-MMM-yyyy") : "-");
                string strEndPeriodDate = (base.EndPeriodDate != null ? base.EndPeriodDate.Value.ToString("dd-MMM-yyyy") : "-");

                return string.Format("{0} To {1}", strStartPeriodDate, strEndPeriodDate);
            
            } 
        
        }
       
        
    }
}
