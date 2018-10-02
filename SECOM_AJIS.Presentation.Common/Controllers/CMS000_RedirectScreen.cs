using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Controllers;
using System.Web.Mvc;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        public ActionResult CMS000()
        {
            return View();
        }

        public ActionResult CMS000_RedirectScreen()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.dsTransData != null)
                {
                    res.ResultData = "/Common/CMS020";

                    doDirectScreen dos = CommonUtil.GetSession<doDirectScreen>("DIRECT_SCREEN");
                    if (dos != null)
                    {
                        res.ResultData = dos;
                        CommonUtil.SetSession("DIRECT_SCREEN", null);
                    }
                }
                else
                    res.ResultData = "/Common/CMS010";
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}
