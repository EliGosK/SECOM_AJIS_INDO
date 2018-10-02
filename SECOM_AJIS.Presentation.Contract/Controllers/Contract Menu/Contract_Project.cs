using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract.CustomEntity;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract;
using System.Collections.Generic;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        [Initialize("CMS020")]
        public ActionResult Contract_Project()
        {
            List<Menu> MenuSession = ViewBag.lstMenu;
            MenuSession = MenuSession[1].SubMenu; //MenuSession[1] is Contract section

            Dictionary<string, Menu> ContractSectionDic = new Dictionary<string, Menu>();
            foreach (Menu item in MenuSession[4].SubMenu) // 4 is Project
                ContractSectionDic[item.Action] = item;

            //Dictionary<string, UserPermissionDataDo> userPermission = new Dictionary<string, UserPermissionDataDo>();
            //foreach (UserPermissionDataDo item in CommonUtil.dsTransData.dtUserPermissionData)
            //    userPermission[item.ObjectID] = item;

            //foreach (Menu item in ContractSectionDic.Values)
            //{
            //    if (false == userPermission.ContainsKey(item.Action))
            //    {
            //        item.isVisible = false;
            //        continue;
            //    }
            //}
            ViewBag.ContractListMenu = ContractSectionDic;
            return View();

        }
    }
}