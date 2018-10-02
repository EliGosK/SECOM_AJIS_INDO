//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;


using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Presentation.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS310
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS310_Authority(CMS310_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            return InitialScreenEnvironment<CMS310_ScreenParameter>("CMS310", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS310
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS310")]
        public ActionResult CMS310()
        {
            return View();
        }

        /// <summary>
        /// Initial grid of screen CMS310
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS310_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS310"));
        }

        /// <summary>
        /// Get contract data list by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS310_SearchResponse(doContractSearchCondition cond)
        {
            ObjectResultData res = new ObjectResultData();

            List<View_dtContractData> nlst = new List<View_dtContractData>();

            CommonUtil c = new CommonUtil();
            
            try
            {
                cond.CustomerCode = c.ConvertCustCode(cond.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                string[] strs = { "Counter" };
                if (CommonUtil.IsNullAllField(cond, strs))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                   
                }
                else
                {

                    //if (cond.Counter == 0)
                    //{
                    //    res.ResultData = CommonUtil.ConvertToXml<View_dtContractData>(nlst, "Common\\CMS310", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    //    return Json(res);
                    //}

                    IViewContractHandler handler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                    List<dtContractData> list = handler.GetContractDataForSearch(cond);
                    //Add Currency
                    for (int i = 0; i < list.Count(); i++)
                    {
                        list[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }

                    foreach (dtContractData l in list)
                    {
                        nlst.Add(CommonUtil.CloneObject<dtContractData, View_dtContractData>(l));
                    }

                    // Select OfficeName_Extra by language
                    nlst = CommonUtil.ConvertObjectbyLanguage<View_dtContractData, View_dtContractData>(nlst, "OfficeName_Extra");

                }

            }
            catch (Exception ex)
            {
                nlst = new List<View_dtContractData>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);

            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtContractData>(nlst, "Common\\CMS310", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);

        }



    }
}
