using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter of CMS430
    /// </summary>
    public class CMS430_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { set; get; }

        [KeepSession]
        public string BillingOCC { set; get; }

        [KeepSession]
        public dtTbt_RentalContractBasicForView dtRentalContract { set; get; }

        [KeepSession]
        public List<dtViewBillingOccList> dtOCCList { set; get; }

        
        public List<dtViewDepositDetailInformation> dtDepositDetail { set; get; }
    }
      
    public class CMS430_Test
    {
        public string OfficeName { set; get; }
        public string Combobox1 { set; get; }
        public DateTime? DateForm { set; get; }
        public DateTime? DateTo { set; get; }
        public DateTime? Date1 { set; get; }
    }
}
