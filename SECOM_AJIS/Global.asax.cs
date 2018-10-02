using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CSI.WindsorHelper;

using SECOM_AJIS.Presentation.Common.Helpers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Controllers;

namespace SECOM_AJIS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801


    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Localization",
                "{lang}/{controller}/{action}/{id}",
                new { lang = "us", controller = "Common", action = "CMS010", id = UrlParameter.Optional } // Parameter defaults
            );

            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Common", action = "CMS020_Home", id = UrlParameter.Optional } // Parameter defaults
            //);

        }
    

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            CommonUtil.WebPath = HttpContext.Current.Server.MapPath("~");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

            Common.Util.ConstantUtil.InitialConstants();
        
            ServiceContainer.Init();
            
            
           // GetScreenName();
        }


        protected void Application_End() // Add by Noprawee P. on 2011 June 30
        {
            //ISchedulerFactory schedFact = new StdSchedulerFactory();
            //// get a scheduler
            //IScheduler sched = schedFact.GetScheduler();
            //sched.Shutdown(false);
        }
        protected void Application_Error()
        {


            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Http404";
            //routeData.Values["exception"] = exception;
            Response.StatusCode = 500;
            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
                switch (Response.StatusCode)
                {
                    case 403:
                        routeData.Values["action"] = "Http404";
                        break;
                    case 404:
                        routeData.Values["action"] = "Http404";
                        break;
                }
            }
            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;
            IController errorsController = new ErrorsController();
            HttpContextWrapper wrapper = new HttpContextWrapper(Context);
            var rc = new RequestContext(wrapper, routeData);
            errorsController.Execute(rc);
        }
    }
}