using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsContractDocForIssue
    {
        public List<dtContractDoc> dtContractDocList { set; get; }
        public List<dtRentalContractBasicForView> dtRentalContractBasicForViewList { set; get; }
        public List<dtSaleContractBasicForView> dtSaleContractBasicForViewList { set; get; }
    }
}
