using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM_AJIS.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult General(Exception exception)
        {
            return View("Exception", exception);
        }

        public ActionResult Http404()
        {
            return View("~/Views/Shared/404.cshtml");
        }

        public ActionResult Http403()
        {
            return View("403");
        }

    }
    public class Http404Controller : Controller
    {
        

        public ActionResult Index()
        {
            return View("404");
        }


    }
}
