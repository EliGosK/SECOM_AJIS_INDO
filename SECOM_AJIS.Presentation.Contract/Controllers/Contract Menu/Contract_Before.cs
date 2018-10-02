using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract.CustomEntity;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        [Initialize("CMS020")]
        public ActionResult Contract_Before()
        {
            List<Menu> MenuSession = ViewBag.lstMenu;
            MenuSession = MenuSession[1].SubMenu; //MenuSession[1] is Contract section

            List<Dictionary<string, Menu>> lines = new List<Dictionary<string, Menu>>();

            Dictionary<string, Menu> dic = null;
            for (int idx = 0; idx < MenuSession[0].SubMenu.Count; idx++)
            {
                if (idx % 3 == 0)
                {
                    dic = new Dictionary<string, Menu>();
                    lines.Add(dic);
                }

                Menu item = MenuSession[0].SubMenu[idx];
                dic.Add(item.MenumKey, item);
            }

            ViewBag.Lines = lines;
            return View("ContractMenu");
        }

    }
}