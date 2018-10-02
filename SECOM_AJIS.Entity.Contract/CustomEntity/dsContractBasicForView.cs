using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsContractBasicForView
    {
        List<dtTbt_RentalContractBasicForView> dtTbt_RentalContractBasicForView { set; get; }
        List<dtTbt_RentalSecurityBasicForView> dtTbt_RentalSecurityBasicForView { set; get; }
        List<dtTbt_RentalMaintenanceDetailsForView> dtTbt_RentalMaintenanceDetailForView { set; get; }
        List<dtTbt_CancelContractMemoDetailForView> dtTbt_CancelContractMemoDetailForView { set; get; }
        List<dtRelatedContract> dtMaintenanceRelatedContract { set; get; }
        List<dtRelatedContract> dtSaleRelatedContract { set; get; }

    }
}
