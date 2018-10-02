using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IDraftContractHandler
    {
        /// <summary>
        /// To search draft contract list for displaying on search screen
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtSearchDraftContractResult> SearchDraftContractList(doSearchDraftContractCondition cond);
    }
}
