//*********************************
// Create by: Natthavat S.
// Create date: 02/FEB/2012
// Update date: 02/FEB/2012
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Transactions;

using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Presentation.Inventory.Models;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check user's permission.
        /// </summary>
        /// <param name="param">Screen's parameter.</param>
        /// <returns>Return ActionResult of the screen.</returns>
        public ActionResult IVS201_Authority(IVS201_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //if (srvCommon.IsSystemSuspending())
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                //    return Json(res);
                //}
                
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_VIEW_INSTRUMENT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS201_ScreenParameter>("IVS201", param, res);
        }

        #endregion

        #region Action
        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ACtionResult of the screen.</returns>
        [Initialize("IVS201")]
        public ActionResult IVS201()
        {
            ViewBag.HeadOfficeCode = this.IVS201_GetHeadOffice();
            return View();
        }

        /// <summary>
        /// Get data for initialize search result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize search result grid.</returns>
        public ActionResult IVS201_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS201_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search instrument data.
        /// </summary>
        /// <param name="param">DO of searching parameter.</param>
        /// <returns>Return ActionResult of JSON data for search result grid.</returns>
        public ActionResult IVS201_GetIVS201(doGetIVS201 param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (param.LocationCode == null
                    && param.AreaCode == null
                    && param.ShelfNoFrom == null
                    && param.ShelfNoTo == null
                    && param.InstrumentCode == null
                    && param.InstrumentName == null
                    && param.OfficeCode == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else if (!string.IsNullOrEmpty(param.ShelfNoFrom)
                    && !string.IsNullOrEmpty(param.ShelfNoTo)
                    && string.Compare(param.ShelfNoFrom, param.ShelfNoTo) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4015, null, new string[] { param.txtSearchShelfNoFrom, param.txtSearchShelfNoTo });
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    var lst = service.GetIVS201(param);
                    res.ResultData = CommonUtil.ConvertToXml<doResultIVS201>(lst, "inventory\\IVS201_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Method

        /// <summary>
        /// Get inventory head office.
        /// </summary>
        /// <returns>Return inventory head office code.</returns>
        private string IVS201_GetHeadOffice()
        {
            var srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            var lstOffice = srvInv.GetInventoryHeadOffice();
            if (lstOffice != null && lstOffice.Count > 0)
            {
                return lstOffice[0].OfficeCode;
            }
            else
            {
                throw new ApplicationException("Unable to get inventory head office data.");
            }
        }

        #endregion
    }
}
