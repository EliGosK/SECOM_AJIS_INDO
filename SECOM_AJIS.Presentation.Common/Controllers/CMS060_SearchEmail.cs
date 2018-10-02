
//*********************************
// Create by: Narupon
// Create date: 17/Jun/2010
// Update date: 17/Jun/2010
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

using SECOM_AJIS.Presentation.Common.Models;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS060
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS060_Authority(CMS060_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS060_ScreenParameter>("CMS060", param, res);
        }

        /// <summary>
        ///  Method for return view of screen CMS060
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS060")]
        public ActionResult CMS060()
        {
            
            return View();
        }

        /// <summary>
        /// Initial grid of screen CMS060
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS060_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS060" ,CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get email data list by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS060_SearchResponse(doEmailSearchCondition cond)
        {

            List<View_dtEmailAddress> nlst = new List<View_dtEmailAddress>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
            

                string[] strs = { "Counter" };
                if (CommonUtil.IsNullAllField(cond, strs))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                }
                else
                {
                    //if (cond.Counter == 0)
                    //{
                    //    res.ResultData = CommonUtil.ConvertToXml<View_dtEmailAddress>(nlst, "Common\\CMS060", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    //    return Json(res);
                    //}

                    IUserHandler handler = ServiceContainer.GetService<IUserHandler>() as IUserHandler;

                    // Execute when press search button and retrive result list
                    List<dtEmailAddress> list = handler.GetUserEmailAddressDataList(cond);


                    //selected launguage
                    list = CommonUtil.ConvertObjectbyLanguage<dtEmailAddress, dtEmailAddress>(list, "OfficeName");

                    nlst = new List<View_dtEmailAddress>();
                    foreach (dtEmailAddress l in list)
                    {
                        /*  For test in case search result more than 1000 rows
                     
                        for (int i = 0; i < 200; i++)
                        {
                            nlst.Add(CommonUtil.CloneObject<dtEmailAddress, View_dtEmailAddress>(l));
                        }
                     
                        */

                        nlst.Add(CommonUtil.CloneObject<dtEmailAddress, View_dtEmailAddress>(l));

                    }
                }


            }
            catch (Exception ex)
            {

                nlst = new List<View_dtEmailAddress>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtEmailAddress>(nlst, "Common\\CMS060" ,CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }


        /// <summary>
        /// Check email suffix
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public ActionResult CMS060_CheckEmailSuffix(List<dtEmailAddress> addr)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Get Email suffix

                string emailSuffix = "";

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> emlst = chandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
                if (emlst.Count > 0)
                    emailSuffix = emlst[0].ConfigValue;

                #endregion

                foreach (dtEmailAddress a in addr)
                {
                    //if (a.EmailAddress.IndexOf(emailSuffix) < 0)
                    if (String.IsNullOrEmpty(a.EmailAddress) == false
                        && a.EmailAddress.ToUpper().IndexOf(emailSuffix.ToUpper()) < 0) //Modify by Jutarat A. on 21112012
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0151);
                        return Json(res);
                    }
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}
