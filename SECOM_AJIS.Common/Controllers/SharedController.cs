using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Common.Controllers
{
    public class SharedController : BaseController
    {
        /// <summary>
        /// Get message data
        /// </summary>
        /// <param name="module"></param>
        /// <param name="code"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMessage(string module, string code, params string[] param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MessageUtil.MessageList msgCode;
                if (Enum.TryParse<MessageUtil.MessageList>(code, out msgCode))
                    res.ResultData = MessageUtil.GetMessage(module, msgCode, param);

      
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get language text
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public ActionResult GetLanguageMessage(string lang)
        {
            ObjectResultData res = new ObjectResultData();
            System.Globalization.CultureInfo cul = System.Threading.Thread.CurrentThread.CurrentUICulture;

            try
            {
                string param = "";
                if (lang == CommonValue.DEFAULT_SHORT_LANGUAGE_JP)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(CommonValue.DEFAULT_LANGUAGE_JP);
                    param = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblJapaneseLanguage");
                    
                }
                else if (lang == CommonValue.DEFAULT_SHORT_LANGUAGE_LC)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    param = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblLocalLanguage");
                }
                else
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    param = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblEnglishLanguage");
                }

                string[] button = new string[]
                {
                    CommonUtil.GetLabelFromResource("Common", "CommonResources", "btnOK"),
                    CommonUtil.GetLabelFromResource("Common", "CommonResources", "btnCancel")
                };

                res.ResultData = new object[]
                {   
                    MessageUtil.GetMessage(
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0103,
                        param),
                    button
                };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = cul;
            }

            return Json(res);
        }
    }
}
