using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Common.Controllers
{
    public class TestShareController : BaseController
    {
        public ActionResult ShareJs()
        {
            return View();
        }

        public ActionResult TestGrid()
        {
            return View("/Test/TestGrid");
        }
        public string TestString()
        {
            return "Test from TestShareController";
        }
    }
}
