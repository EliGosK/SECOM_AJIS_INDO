
//*********************************
// Create by: Waroon H.
// Create date: 29/Mar/2012
// Update date: 29/Mar/2012
//*********************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Presentation.Income.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS120_Authority(ICS120_ScreenParameter param)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            ObjectResultData res = new ObjectResultData();

            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            // Check User Permission
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_MATCH_WHT, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<ICS120_ScreenParameter>("ICS120", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS120")]
        public ActionResult ICS120()
        {
            return View();
        }

        /// <summary>
        /// Get data for initialize match payment detail grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data data for initialize match payment detail grid.</returns>
        public ActionResult ICS120_InitialSearchIncomeWHT()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS120_SearchIncomeWHT", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        public ActionResult ICS120_SearchIncomeWHT(doIncomeWHTSearchCriteria param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                if (param == null)
                {
                    param = new doIncomeWHTSearchCriteria();
                }

                var list = hand.SearchIncomeWHT(param);
                if (list != null & list.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage(list);
                }
                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS120_SearchIncomeWHT", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

    }
}
