using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;
using System.Xml;
using SECOM_AJIS.Presentation.ExchangeRate.Models;
using SECOM_AJIS.DataEntity.ExchangeRate;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.ExchangeRate.Handlers;
using SECOM_AJIS.RateCalcuration.Util;
using SECOM_AJIS.DataEntity.ExchangeRate.ConstantValue;
using SECOM_AJIS.Entity.ExchangeRate;

namespace SECOM_AJIS.Presentation.ExchangeRate.Controllers
{
    public partial class ExchangeRateController : BaseController
    {
        public ActionResult Master_Authority(Master_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
           
            //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DEBT_TRACING, FunctionID.C_FUNC_ID_OPERATE))
            //{
            //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
            //    return Json(res);
            //}

            return InitialScreenEnvironment<Master_ScreenParameter>("Master", param, res);
        }

        [Initialize("Master")]
        public ActionResult Master()
        {
            double errorCode = RateCalcCode.C_NO_ERROR;

            //double amount1 = 10000000000;
            //RateCalc.doCalc(DateTime.Now.Date, RateCalcCode.C_CONVERT_TYPE_TO_RPIAH, amount1, ref amount1, ref errorCode);
            
            //double amount2 = 100;
            //RateCalc.doCalc(DateTime.Now.Date, RateCalcCode.C_CONVERT_TYPE_TO_DOLLAR, amount2, ref amount2, ref errorCode);

            string baseDateStr = DateTime.Now.ToString("ddMMyyyy");
            JsonResult res = GetExchangeBankRate(baseDateStr, ref errorCode) as JsonResult;
            ObjectResultData obj = res.Data as ObjectResultData;
            ViewBag.rate = obj.ResultData;

            return View();
        }

        public ActionResult GetCalendarItems(string start, string end)
        {
            ExchangeRateHandler handler = new ExchangeRateHandler();

            return Json(handler.GetAllExchangeRateForCalendar());
        }

        public ActionResult Detail_Authority(Master_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DEBT_TRACING, FunctionID.C_FUNC_ID_OPERATE))
            //{
            //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
            //    return Json(res);
            //}

            return InitialScreenEnvironment<Master_ScreenParameter>("Detail", param, res);
        }
        
        public ActionResult Detail()
        {
            return View();
        }

        public ActionResult RegisterRate()
        {
            ExchangeRateHandler handler = new ExchangeRateHandler();
            return Json(handler.RegisterRate(Request.Form));
        }

        public JsonResult GetCurrentExchangeRate()
        {
            ExchangeRateHandler handler = new ExchangeRateHandler();
            return Json(handler.GetCurrentExchangeRate(DateTime.Now.Date));
        }

        public JsonResult GetExchangeRateByTargetDate()
        {
            ExchangeRateHandler handler = new ExchangeRateHandler();
            string baseDateStr = Request.Form["targetDate"];
            DateTime? targetDate = DateTime.ParseExact(baseDateStr, "d/MMM/yyyy", null);
            return Json(handler.GetExchangeRateByTargetDate(targetDate));
        }
    }
}
