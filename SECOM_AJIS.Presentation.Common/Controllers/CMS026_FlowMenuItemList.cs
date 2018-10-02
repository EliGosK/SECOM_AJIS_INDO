using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Controllers;
using System.Web.Mvc;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS026_Authority(CMS026_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                #region Validate data

                if (CommonUtil.IsNullOrEmpty(param.PopupSubMenuID))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS026_ScreenParameter>("CMS026", param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS026")]
        public ActionResult CMS026()
        {
            try
            {
                CMS026_ScreenParameter param = GetScreenObject<CMS026_ScreenParameter>();

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doPopupSubMenuList> lst = chandler.GetPopupSubMenuList(param.PopupSubMenuID);

                ViewBag.PopupSubMenuList = lst;

                if (lst.Count > 0)
                    ViewBag.PopupSubmenuName = lst[0].PopupSubmenuName;
            }
            catch (Exception)
            {
                ViewBag.PopupSubMenuList = new List<doPopupSubMenuList>();
            }

            return View();
        }

        #endregion
    }
}
