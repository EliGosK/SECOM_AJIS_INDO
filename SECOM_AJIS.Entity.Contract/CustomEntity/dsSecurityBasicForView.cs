using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsSecurityBasicForView
    {
        List<dtTbt_RentalContractBasicForView> dtTbt_RentalContractBasicForView { set; get; }
        List<dtTbt_RentalSecurityBasicForView> dtTbt_RentalSecurityBasicForView { set; get; }
        List<dtTbt_RentalOperationTypeListForView> dtTbt_RentalOperationTypeListForView { set; get; }
        List<dtContractsSameSite> dtContractsSameSite { set; get; }
        List<dtTbt_RentalInstSubContractorListForView> dtTbt_RentalInstSubContractorListForView { set; get; }

    }
}
