using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SECOM_AJIS.DataEntity.Quotation;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using System.Diagnostics;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Controllers
{
    public class SwtIVP100Controller : SwtCommonController
    {
        //
        // GET: /SwtIVP100/

        public string Index()
        {
            string result = "";

            result = ExecuteProcess();

            return result;
        }

        ///<summary>
        ///Purpose   : Process result
        ///Parameters: None
        ///Expected  : Result table in db.
        ///</summary>
        ///
        public string ExecuteProcess()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            doCommonCSVInv result = null;
            string strRes = "";
            bool isValid = false;

            string strFormat_output2 = "AccountCode: {0} || AreaCode: {1} || TotalSum: {2}";

            try
            {
                //Spec changed. Can't test by this method.
                //result = invenhandler.GenerateInventoryAccountData(CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                List<string> resultLst = new List<string>();

                foreach (var item in result.AssetAmountAcc)
                {
                    resultLst.Add(string.Format(strFormat_output2, item.AccountCode, item.AreaCode, item.TotalSum));
                }

                isValid = true;
                resultLst.Add(String.Format(RESULT_FORMAT_LIST, 1, (isValid) ? "Process completed." : strRes));

                strRes = CommonUtil.TextList(resultLst.ToArray(), "<br />");
            }
            catch (Exception ex)
            {
                strRes = ex.StackTrace;
            }

            return strRes;
        }
    }
}
