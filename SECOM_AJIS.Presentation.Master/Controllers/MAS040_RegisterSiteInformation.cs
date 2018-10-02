//*********************************
// Create by: Attawhit  Chuoosathan
// Create date: 29/Jun/2010
// Update date: 29/Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Master.Models;
using System.Reflection;



namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS040_Screen = "MAS040";

        #region Authority

        /// <summary>
        /// - Check user permission for screen MAS010.<br />
        /// - Check system suspending.<br />
        /// - Get building usage from tbm_BuildingUsage.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS040_Authority(MAS040_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_BuildingUsage> ulst = mhandler.GetTbm_BiuldingUsage();
                if(ulst != null && ulst.Count > 0)
                {
                    param.tbm_BuildingUsageList = ulst;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS040_ScreenParameter>("MAS040", param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS040_Screen)]
        public ActionResult MAS040()
        {
            try
            {
                ViewBag.IsHideSiteCode = false;
                ViewBag.CallValidateMethod = false;

                MAS040_ScreenParameter siteData = GetScreenObject<MAS040_ScreenParameter>();
                if (siteData != null)
                {
                    if (siteData.CallerScreenID == SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
                        ViewBag.IsHideSiteCode = true;

                    doSite doSite = siteData.doSite;
                    if (doSite != null)
                    {
                        ViewBag.SiteCode = doSite.SiteCodeShort;
                        ViewBag.SiteNameEN = doSite.SiteNameEN;
                        ViewBag.SiteNameLC = doSite.SiteNameLC;
                        ViewBag.SECOMContactPerson = doSite.SECOMContactPerson;
                        ViewBag.PersonInCharge = doSite.PersonInCharge;
                        ViewBag.PhoneNo = doSite.PhoneNo;
                        ViewBag.UsageCode = doSite.BuildingUsageCode;
                        ViewBag.AddressEN = doSite.AddressEN;
                        ViewBag.AddressLC = doSite.AddressLC;
                        ViewBag.AlleyEN = doSite.AlleyEN;
                        ViewBag.AlleyLC = doSite.AlleyLC;
                        ViewBag.RoadEN = doSite.RoadEN;
                        ViewBag.RoadLC = doSite.RoadLC;
                        ViewBag.SubDistrictEN = doSite.SubDistrictEN;
                        ViewBag.SubDistrictLC = doSite.SubDistrictLC;
                        ViewBag.ProvinceEN = doSite.ProvinceCode;
                        ViewBag.ProvinceLC = doSite.ProvinceCode;
                        ViewBag.DistrictEN = doSite.DistrictCode;
                        ViewBag.DistrictLC = doSite.DistrictCode;
                        ViewBag.ZipCode = doSite.ZipCode;

                        if (doSite.ValidateSiteData == false)
                            ViewBag.CallValidateMethod = true;
                    }
                }
            }
            catch(Exception)
            {
            }

            return View();
        }

        #endregion
        #region Actions

        /// <summary>
        /// Validate user inputed data.<br />
        /// - Set usage.<br />
        /// - Set province.<br />
        /// - Set district.<br />
        /// - Check require field.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS040_ValidateData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                doSite doSite = null;

                MAS040_ScreenParameter siteData = GetScreenObject<MAS040_ScreenParameter>();
                if (siteData != null)
                {
                    if (siteData.doSite != null)
                        doSite = siteData.doSite;
                }

                if (doSite != null)
                {
                    IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                    #region Usage

                    if (CommonUtil.IsNullOrEmpty(doSite.BuildingUsageCode) == false)
                    {
                        List<tbm_BuildingUsage> ulst = mhandler.GetTbm_BiuldingUsage();
                        if (ulst.Count > 0)
                        {
                            foreach (tbm_BuildingUsage u in ulst)
                            {
                                if (doSite.BuildingUsageCode == u.BuildingUsageCode)
                                {
                                    doSite.BuildingUsageName = u.BuildingUsageName;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                    #region Province Data

                    if (CommonUtil.IsNullOrEmpty(doSite.ProvinceCode) == false)
                    {
                        List<tbm_Province> plst = mhandler.GetTbm_Province();
                        if (plst.Count > 0)
                        {
                            foreach (tbm_Province pv in plst)
                            {
                                if (doSite.ProvinceCode == pv.ProvinceCode)
                                {
                                    doSite.ProvinceNameEN = pv.ProvinceNameEN;
                                    doSite.ProvinceNameLC = pv.ProvinceNameLC;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                    #region District

                    if (CommonUtil.IsNullOrEmpty(doSite.DistrictCode) == false)
                    {
                        List<tbm_District> dlst = mhandler.GetTbm_District(doSite.ProvinceCode);
                        if (dlst.Count > 0)
                        {
                            foreach (tbm_District d in dlst)
                            {
                                if (doSite.ProvinceCode == d.ProvinceCode
                                    && doSite.DistrictCode == d.DistrictCode)
                                {
                                    doSite.DistrictNameEN = d.DistrictNameEN;
                                    doSite.DistrictNameLC = d.DistrictNameLC;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                }

                MAS040_ValidateCombo validate = CommonUtil.CloneObject<doSite, MAS040_ValidateCombo>(doSite);
                ValidatorUtil.BuildErrorMessage(res, new object[] { validate });

                if (doSite != null)
                {
                    if (doSite.ValidateSiteData == false)
                    {
                        MAS040_CheckRequiredField cSiteDo = CommonUtil.CloneObject<doSite, MAS040_CheckRequiredField>(doSite);
                        /*
                        if (CommonUtil.IsNullOrEmpty(cSiteDo.BuildingUsageName))
                            cSiteDo.BuildingUsageCode = null;
                        */
                        if ((CommonUtil.IsNullOrEmpty(cSiteDo.ProvinceCode) == false)
                            && (CommonUtil.IsNullOrEmpty(cSiteDo.ProvinceNameEN))
                            && (CommonUtil.IsNullOrEmpty(cSiteDo.ProvinceNameLC)))
                        {
                            cSiteDo.ProvinceNameEN = cSiteDo.ProvinceCode;
                            cSiteDo.ProvinceNameLC = cSiteDo.ProvinceCode;
                        }

                        if ((CommonUtil.IsNullOrEmpty(cSiteDo.DistrictCode) == false)
                            && (CommonUtil.IsNullOrEmpty(cSiteDo.DistrictNameEN))
                            && (CommonUtil.IsNullOrEmpty(cSiteDo.DistrictNameLC)))
                        {
                            cSiteDo.DistrictNameEN = cSiteDo.DistrictCode;
                            cSiteDo.DistrictNameLC = cSiteDo.DistrictCode;
                        }
                        
                        ValidatorUtil.BuildErrorMessage(res, new object[] { cSiteDo });
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get SpecialCareFlag from tbm_BuildingUsageList in ScreenParameter that match to UsageCode
        /// </summary>
        /// <param name="UsageCode"></param>
        /// <returns></returns>
        public ActionResult MAS040_GetAttachImportanceFlag(string UsageCode)
        {
            IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            MAS040_ScreenParameter screenParam = GetScreenObject<MAS040_ScreenParameter>();
            if (CommonUtil.IsNullOrEmpty(UsageCode) == false)
            {
                //List<tbm_BuildingUsage> ulst = mhandler.GetTbm_BiuldingUsage();
                if (screenParam.tbm_BuildingUsageList.Count > 0)
                {
                    foreach (tbm_BuildingUsage u in screenParam.tbm_BuildingUsageList)
                    {
                        if (UsageCode == u.BuildingUsageCode)
                        {
                            res.ResultData = u.SpecialCareFlag;
                            break;
                        }
                    }
                   
                }
            }
            return Json(res);
        }

        /// <summary>
        /// Confirm data user input.<br />
        /// - Generate customer full address.<br />
        /// - Check is site change.
        /// </summary>
        /// <param name="doSite"></param>
        /// <returns></returns>
        public ActionResult MAS040_ConfirmData(MAS040_CheckRequiredField doSite)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                #region Create Customer Address Full

                doCustomer cust = CommonUtil.CloneObject<doSite, doCustomer>(doSite);
                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                mhandler.CreateAddressFull(cust);

                doSite.AddressFullEN = cust.AddressFullEN;
                doSite.AddressFullLC = cust.AddressFullLC;

                #endregion
                #region Check is changed

                if (doSite != null)
                {
                    if (MAS040_IsSiteChanged(doSite) == true)
                        doSite.SiteCode = null;
                    else
                    {
                        CommonUtil cmm = new CommonUtil();
                        doSite.SiteCode = cmm.ConvertSiteCode(doSite.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }
                }
                #endregion

                res.ResultData = doSite;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Methods

        private bool MAS040_IsSiteChanged(doSite siteDo)
        {
            try
            {
                doSite oSiteDo = null;
                MAS040_ScreenParameter siteData = GetScreenObject<MAS040_ScreenParameter>();
                if (siteData != null)
                {
                    if (siteData.doSite != null)
                        oSiteDo = siteData.doSite;
                }

                if (oSiteDo != null)
                {
                    if (CommonUtil.IsNullOrEmpty(oSiteDo.SiteCode))
                        return true;
                }
                if (oSiteDo != null && siteDo != null)
                {
                    bool isSame = true;
                    List<string> chkPropLst = new List<string>() 
                    { 
                        "SiteNameEN", 
                        "SiteNameLC", 
                        "SECOMContactPerson", 
                        "PersonInCharge",
                        "PhoneNo",
                        "UsageCode",
                        "AddressEN",
                        "AlleyEN",
                        "RoadEN",
                        "SubDistrictEN",
                        "ProvinceEN",
                        "DistrictEN",
                        "AddressLC",
                        "AlleyLC",
                        "RoadLC",
                        "SubDistrictLC",
                        "ProvinceLC",
                        "DistrictLC",
                        "Zipcode"
                    };
                    PropertyInfo[] props = typeof(doSite).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.CanWrite == false)
                            continue;
                        if (chkPropLst.IndexOf(prop.Name) < 0)
                            continue;

                        object obj1 = prop.GetValue(oSiteDo, null);
                        object obj2 = prop.GetValue(siteDo, null);

                        if (CommonUtil.IsNullOrEmpty(obj1) == false || CommonUtil.IsNullOrEmpty(obj2) == false)
                        {
                            isSame = false;
                            if (CommonUtil.IsNullOrEmpty(obj1) == false && CommonUtil.IsNullOrEmpty(obj2) == false)
                            {
                                if (obj1.ToString() == obj2.ToString())
                                    isSame = true;
                            }

                            if (isSame == false)
                                break;
                        }
                    }

                    if (isSame == false)
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}