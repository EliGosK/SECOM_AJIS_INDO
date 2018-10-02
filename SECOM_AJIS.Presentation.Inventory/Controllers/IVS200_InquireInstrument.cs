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
        /// Checking user's permission.
        /// </summary>
        /// <param name="param">Screen's parameter</param>
        /// <returns>Return the ActionResult of the screen.</returns>
        public ActionResult IVS200_Authority(IVS200_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                //if (commonhandler.IsSystemSuspending())
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                //    return Json(res);
                //}

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_INQUIRE_INSTRUMENT))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                    return Json(res);
                }

                IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                var headOffice = invenhandler.GetInventoryHeadOffice();
                if (headOffice == null || headOffice.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4016);
                    //res.ResultData = MessageUtil.MessageList.MSG0049.ToString();
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS200_ScreenParameter>("IVS200", param, res);
        }

        #endregion

        #region Action
        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return the ActionResult of the screen.</returns>
        [Initialize("IVS200")]
        public ActionResult IVS200()
        {
            return View();
        }

        /// <summary>
        /// Get data for initialize instrument quantity grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize instrument quantity grid.</returns>
        public ActionResult IVS200_InitialInstrumentQuantityGrid()
        {
            return Json(CommonUtil.ConvertToXml<doResultIVS200>(new List<doResultIVS200>(), "Inventory\\IVS200_InstrumentQuantity", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        public ActionResult IVS200_InitialInstrumentQuantityGridDetail()
        {
            return Json(CommonUtil.ConvertToXml<doInventoryBookingDetail>(new List<doInventoryBookingDetail>(), "Inventory\\IVS200_DetailInstrumentQuantity", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get data for initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize screen.</returns>
        public ActionResult IVS200_RetrieveInitialData()
        {
            ObjectResultData res = new ObjectResultData();
            IVS200_InitialData obj = new IVS200_InitialData();

            try
            {
                IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                var headOffice = invenhandler.GetInventoryHeadOffice();
                if (headOffice.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<doOffice>(headOffice);
                    obj.OfficeCode = headOffice[0].OfficeCode;
                    obj.OfficeName = headOffice[0].OfficeName;
                }

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                miscs.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_INV_LOC,
                    ValueCode = InstrumentLocation.C_INV_LOC_INSTOCK
                });

                var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                if (outlst.Count == 1)
                {
                    CommonUtil.MappingObjectLanguage<doMiscTypeCode>(outlst);
                    obj.Location = outlst[0].ValueDisplay;
                }

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search instrument data.
        /// </summary>
        /// <param name="cond">DO for search instrument data.</param>
        /// <returns>Return ActionResult of JSON data for instrument quantity list.</returns>
        public ActionResult IVS200_RetrieveInstrumentQuantityList(doGetIVS200 cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region //R2
                IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                res.ResultData = CommonUtil.ConvertToXml<doResultIVS200>(invenhandler.GetIVS200(cond), "Inventory\\IVS200_InstrumentQuantity", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                #endregion
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult IVS200_RetrieveInstrumentQuantityListDetail(doGetIVS200_Detail cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region //R2
                if (cond != null)
                {
                    IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var listdata = invenhandler.GetIVS200_Detail(cond);
                    if (listdata.Count != 0)
                    {
                        res.ResultData = CommonUtil.ConvertToXml<doInventoryBookingDetail>(listdata, "Inventory\\IVS200_DetailInstrumentQuantity", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                    }
                }
                
                #endregion
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

        #endregion
    }
}
