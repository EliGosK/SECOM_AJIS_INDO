using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Master.Handlers;
using SECOM_AJIS.Presentation.Master.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS071_Screen = "MAS071";

        public ActionResult MAS071_Authority(MAS071_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                //    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_ADD) == true
                //    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                //    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_DEL) == true
                //    ))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                //Check system suspending
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS071_ScreenParameter>(MAS071_Screen, param, res);
        }
        
        [Initialize(MAS071_Screen)]
        public ActionResult MAS071()
        {
            return View();
        }

        public ActionResult UpdatePassword()
        {
            PasswordHandler hand = new PasswordHandler();
            return Json(hand.UpdatePassword(Request.Form["oldPassword"], Request.Form["newPassword"]));
        }
    }
}
