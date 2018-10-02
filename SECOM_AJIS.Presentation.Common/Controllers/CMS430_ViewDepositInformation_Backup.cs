

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        //public ActionResult CMS430_Authority(CMS430_ScreenParameter param)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
              
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return InitialScreenEnvironment<CMS430_ScreenParameter>("CMS430", param, res);
        //}

        //[Initialize("CMS430")]
        //public ActionResult CMS430()
        //{
        //    return View();
        //}


        //public ActionResult CMS430_InitialGrid()
        //{
        //    return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS430_Test", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        //}


        //public ActionResult CMS430_SearchResponse()
        //{

        //    List<CMS430_Test> list = new List<CMS430_Test>();

        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

        //    try
        //    {
        //        CMS430_Test item;
        //        for (int i = 0; i < 5; i++)
        //        {
        //            item = new CMS430_Test();
        //            item.OfficeName = "Test data "+(i+1).ToString();
        //            item.DateForm = DateTime.Now;
        //            item.DateTo = DateTime.Now.AddDays(5);
        //            item.Date1 = DateTime.Now;
        //            list.Add(item);
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        //        res.AddErrorMessage(ex);
        //    }

        //    res.ResultData = CommonUtil.ConvertToXml<CMS430_Test>(list, "Common\\CMS430_Test", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
        //    return Json(res);


        //}

        //public ActionResult CMS430_GetComboBoxPaymentMethod(string id)
        //{
        //    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
        //    try
        //    {
        //        List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
        //        {
        //            new doMiscTypeCode()
        //            {
        //                FieldName = MiscType.C_PAYMENT_METHOD,
        //                ValueCode = "%"
        //            }
        //        };

        //        ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        lst = hand.GetMiscTypeCodeList(miscs);

        //        ComboBoxModel cboModel = new ComboBoxModel();
        //        cboModel.SetList<doMiscTypeCode>(lst, "ValueCodeDisplay", "ValueCode");
        //        return Json(cboModel);
        //    }
        //    catch(Exception  ex)
        //    {
        //        ObjectResultData res = new ObjectResultData();
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }

        //    //if (lst == null)
        //    //    lst = new List<doMiscTypeCode>();

        //    //return Json(CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", null, false).ToString());
        //}
    }
}
