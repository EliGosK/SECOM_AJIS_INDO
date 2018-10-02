using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        /// <summary>
        /// Get name of Shelf data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetShelfName(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doShelfName> lst = handler.GetShelfName(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.ShelfName);
                }
                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
    }
}
