using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.ExchangeRate.Handlers;
using SECOM_AJIS.Presentation.ExchangeRate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SECOM_AJIS.Presentation.ExchangeRate.Controllers
{
    public partial class ExchangeRateController : BaseController
    {
        
        public ActionResult GetExchangeBankRate(string strTargetDate, ref double errorCode)
        {
            if (CommonUtil.IsNullOrEmpty(strTargetDate))
            {
                return Json(string.Empty);
            }
            string dateString = strTargetDate.Replace("-", "");

            DateTime? targetDate = DateTime.ParseExact(dateString, "ddMMyyyy", null);
            if (CommonUtil.IsNullOrEmpty(targetDate))
            {
                return Json(string.Empty);
            }

            ObjectResultData res = new ObjectResultData();
            ConvertExchangeRateHandler hand = new ConvertExchangeRateHandler();
            res.ResultData = hand.GetExchangeBankRate((DateTime)targetDate, ref errorCode);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExchangeTaxRate(string strTargetDate, ref double errorCode)
        {
            if (CommonUtil.IsNullOrEmpty(strTargetDate))
            {
                return Json(string.Empty);
            }
            string dateString = strTargetDate.Replace("-", "");

            DateTime? targetDate = DateTime.ParseExact(dateString, "ddMMyyyy", null);
            if (CommonUtil.IsNullOrEmpty(targetDate))
            {
                return Json(string.Empty);
            }

            ObjectResultData res = new ObjectResultData();
            ConvertExchangeRateHandler hand = new ConvertExchangeRateHandler();
            res.ResultData = hand.GetExchangeTaxRate((DateTime)targetDate, ref errorCode);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConvertAmountbyBankRate(string paramDate, string convertType, decimal amount, ref double errorCode)
        {
            if (CommonUtil.IsNullOrEmpty(paramDate))
            {
                return Json(string.Empty);
            }
            string dateString = paramDate.Replace("-", "");

            DateTime? baseDate = DateTime.ParseExact(dateString, "ddMMyyyy", null);
            if (CommonUtil.IsNullOrEmpty(baseDate))
            {
                return Json(string.Empty);
            }

            ObjectResultData res = new ObjectResultData();
            ConvertExchangeRateHandler hand = new ConvertExchangeRateHandler();
            res.ResultData = (decimal)hand.ConvertAmountByBankRate((DateTime)baseDate, convertType, amount, ref errorCode);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConvertAmountbyTaxRate(string paramDate, string convertType, decimal amount, ref double errorCode)
        {
            if (CommonUtil.IsNullOrEmpty(paramDate))
            {
                return Json(string.Empty);
            }
            string dateString = paramDate.Replace("-", "");

            DateTime? baseDate = DateTime.ParseExact(dateString, "ddMMyyyy", null);
            if (CommonUtil.IsNullOrEmpty(baseDate))
            {
                return Json(string.Empty);
            }

            ObjectResultData res = new ObjectResultData();
            ConvertExchangeRateHandler hand = new ConvertExchangeRateHandler();
            res.ResultData = (decimal)hand.ConvertAmountByTaxRate((DateTime)baseDate, convertType, amount, ref errorCode);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}
