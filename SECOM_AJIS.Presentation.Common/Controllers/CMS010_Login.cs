using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Master.Handlers;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {

        /// <summary>
        /// Initial login screen (CMS010)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS010()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                IMasterHandler handMAS = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_Object> lst = handMAS.GetTbm_Object();
                //List<View_tbm_Object> dtTbm_Object = CommonUtil.ConvertObjectbyLanguage<tbm_Object, View_tbm_Object>(lst, "ObjectName", "ObjectDescription");
                //List<MenuName> MenuName = CommonUtil.ClonsObjectList<View_tbm_Object, MenuName>(dtTbm_Object);


                CommonUtil.MenuNameList = CommonUtil.ClonsObjectList<tbm_Object, MenuName>(lst);
                CommonUtil.MappingObjectLanguage<MenuName>(CommonUtil.MenuNameList);

                List<tbm_Object> cms010Name = (from c in lst where c.ObjectID == "CMS010" select c).ToList<tbm_Object>();

                // Natthavat S. Change title for login screen
                if (cms010Name.Count > 0)
                    ViewBag.Title = String.Format("{0} [SIMS]", cms010Name[0].ObjectNameEN); //Modify by Jutarat A. on 21022013
                    //ViewBag.Title = String.Format("{0} [SECOM-AJIS]", cms010Name[0].ObjectNameEN); 
                    ////ViewBag.Title = CommonUtil.TextCodeName(cms010Name[0].ObjectID, cms010Name[0].ObjectNameEN, "-");
                else
                    ViewBag.Title = "Login [SIMS]"; //Modify by Jutarat A. on 21022013
                    //ViewBag.Title = "Login [SECOM-AJIS]";
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return View();
        }

        /// <summary>
        /// Check username and password.<br />
        /// Check is Employee No. active.<br />
        /// Get user default language.<br/>
        /// Refresh user data.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult Login(doLogin Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                //1.2
                ILoginHandler handLogin = ServiceContainer.GetService<ILoginHandler>() as ILoginHandler;
                bool bLogDomain = handLogin.LoginDomain(Cond);
                //if (!bLogDomain)
                //{
                //    if (handLogin.IsLockedEmployee(Cond.EmployeeNo))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0PWL);
                //    }
                //    else
                //    {
                //        //Comment for Test Only : Nattapong N.
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0099);
                //    }
                //    ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                //    handLog.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, LogMessage.C_LOG_INVALID_USER, EventID.C_EVENT_ID_LOGIN_FAIL);

                //    return Json(res);
                //}

                // Clear Temporary pdf file where crate date < today
                bool isClear = ReportUtil.ClearTemporaryFile();

                //1.3 
                IEmployeeMasterHandler handEmp = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                bool blnExistEmployee = handEmp.CheckExistActiveEmployee(Cond.EmployeeNo);
                //if (!blnExistEmployee)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0099);
                //    return Json(res);
                //}


                dsTransDataModel dsTrans = new dsTransDataModel();
                ITransDataHandler handTrans = ServiceContainer.GetService<ITransDataHandler>() as ITransDataHandler;

                #region Language

                dsTrans.dtTransHeader = new TransHeaderDo();
                dsTrans.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_EN;

                if (this.RouteData.Values["lang"] != null &&
                    !string.IsNullOrWhiteSpace(this.RouteData.Values["lang"].ToString()))
                {
                    // set the culture from the route data (url)
                    string lang = this.RouteData.Values["lang"].ToString();

                    if (lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_EN)
                        dsTrans.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_EN;
                    else if (lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_JP)
                        dsTrans.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_JP;
                    else if (lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_LC)
                        dsTrans.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_LC;
                }
                CommonUtil.dsTransData = dsTrans;

                #endregion
                #region Refesh Data

                handTrans.RefreshUserData(dsTrans, Cond.EmployeeNo);
                handTrans.RefreshOfficeData(dsTrans);
                handTrans.RefreshPermissionData(dsTrans);

                #endregion

                handLogin.KeepHistory(Cond.EmployeeNo, LogType.C_LOG_IN);

                dsTrans.dtOperationData = new OperationDataDo();
                dsTrans.dtOperationData.ProcessDateTime = DateTime.Now;

                dsTrans.dtCommonSearch = new CommonSearchDo();

                CommonUtil.dsTransData = dsTrans;


                //-----------Add by Narupon W.-------(Menu list)-----------------//
                
                IMasterHandler handMAS = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_Object> menuList = handMAS.GetTbm_Object();
                dsTrans.dtMenuNameList = CommonUtil.ClonsObjectList<tbm_Object, MenuName>(menuList);
                CommonUtil.MappingObjectLanguage<MenuName>(dsTrans.dtMenuNameList);

                //-----------END Add by Narupon W.-------(Menu list)-------------//



                res.ResultData = "/Common/CMS020";

                doDirectScreen dos = CommonUtil.GetSession<doDirectScreen>("DIRECT_SCREEN");
                if (dos != null)
                {
                    res.ResultData = dos;
                    CommonUtil.SetSession("DIRECT_SCREEN", null);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// If valid goto Home screen (CMS020).
        /// If invalid return false.
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS010_CHECK()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.dsTransData != null)
                    res.ResultData = "/Common/CMS020";
                else
                    res.ResultData = false;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

    }
}
