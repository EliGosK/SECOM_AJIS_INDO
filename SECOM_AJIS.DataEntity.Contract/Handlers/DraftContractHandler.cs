using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class DraftContractHandler : BizCTDataEntities, IDraftContractHandler
    {
        /// <summary>
        /// To search draft contract list for displaying on search screen
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtSearchDraftContractResult> SearchDraftContractList(doSearchDraftContractCondition cond)
        {
            try
            {
               List<dtSearchDraftContractResult> lst =  base.SearchDraftContractList(cond.QuotationCode,
                                                                 cond.Alphabet,
                                                                 cond.RegistrationDateFrom,
                                                                 cond.RegistrationDateTo,
                                                                 cond.Salesman1Code,
                                                                 cond.Salesman1Name,
                                                                 cond.ContractTargetName,
                                                                 cond.SiteName,
                                                                 cond.ContractOfficeCode,
                                                                 cond.OperationOfficeCode,
                                                                 cond.ApproveContractStatus,
                                                                 cond.ApproveDateFrom,
                                                                 cond.ApproveDateTo);
               if (lst == null)
                   lst = new List<dtSearchDraftContractResult>();
               else
                   CommonUtil.MappingObjectLanguage<dtSearchDraftContractResult>(lst);
               
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
