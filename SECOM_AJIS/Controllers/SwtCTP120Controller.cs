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

namespace SECOM_AJIS.Controllers
{
    public class SwtCTP120Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP120/

        public string index()
        {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Generate AR approv  no. of Approve
        ///Parameters: strARInteractionType = 03
        ///Expected  : APV-12-09001
        ///</summary>
        public string Case1()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARInteractionType = "03";

            string expected = "APV-12-09001";
            string actual;
            
            try
            {
                actual = target.GenerateARApproveNo(strARInteractionType);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate AR approv  no. of Instruction
        ///Parameters: strARInteractionType = 04
        ///Expected  : RJC-12-02001
        ///</summary>
        public string Case2()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARInteractionType = "04";

            string expected = "RJC-12-02001";
            string actual;

            try
            {
                actual = target.GenerateARApproveNo(strARInteractionType);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate AR approv  no. of Reject
        ///Parameters: strARInteractionType = 05
        ///Expected  : ORD-12-03001
        ///</summary>
        public string Case3()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARInteractionType = "05";

            string expected = "ORD-12-03001";
            string actual;

            try
            {
                actual = target.GenerateARApproveNo(strARInteractionType);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate AR approve no. of Reject (How does the system performs if it over maximum of running no.)
        ///Parameters: strARInteractionType = 5
        ///Expected  : MSG3018: Cannot generate approve no. because the number reach maximum, please contact administrator.
        ///</summary>
        public string Case4()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARInteractionType = "05";

            string expected = "MSG3018";
            string actual;

            try
            {
                actual = target.GenerateARApproveNo(strARInteractionType);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

    }
}
