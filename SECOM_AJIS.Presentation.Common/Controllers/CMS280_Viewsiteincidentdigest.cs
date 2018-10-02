//*********************************
// Create by: Nattapong N.
// Create date: 8/Jul/2010
// Update date: 11/Jul/2010
//*********************************


using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check user permission for screen CMS280.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS280_Authority(CMS280_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_SITE_INFO, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                if (CommonUtil.IsNullOrEmpty(param.strSiteCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }

                // ---- Wrong !! -- You must get these data when pass check Authority !!!
                //dsSiteInfoForView dsSiteInfo = GetSiteInfoForView(new CommonUtil().ConvertSiteCode(Cond.strSiteCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                //Cond.dsSiteIfoForView = dsSiteInfo;

                return InitialScreenEnvironment<CMS280_ScreenParameter>("CMS280", param ,res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen and get Site Information.
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS280")]
        public ActionResult CMS280()
        {
            ObjectResultData res = new ObjectResultData();
            CMS280_ScreenParameter param = GetScreenObject<CMS280_ScreenParameter>();
            CommonUtil cm = new CommonUtil();

            string strSiteCode = param.strSiteCode;

            //if (param.dsSiteIfoForView == null)
            //{
            //    param.dsSiteIfoForView = new dsSiteInfoForView();
            //}

            string strSiteCode_long = cm.ConvertSiteCode(param.strSiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            dsSiteInfoForView dsSiteInfo = GetSiteInfoForView(strSiteCode_long);

            param.dsSiteIfoForView = dsSiteInfo;

            if (dsSiteInfo.dtSiteData != null)
            {
                if (dsSiteInfo.dtSiteData.Count > 0)
                {
                    ViewBag.CustCode = dsSiteInfo.dtSiteData[0].CustCode_Short;
                    ViewBag.CustNameEN = dsSiteInfo.dtSiteData[0].CustFullNameEN;
                    ViewBag.CustNameLC = dsSiteInfo.dtSiteData[0].CustFullNameLC;
                    ViewBag.ContPersonName = dsSiteInfo.dtSiteData[0].CustContactPersonName;
                    ViewBag.SecomContPerson = dsSiteInfo.dtSiteData[0].CustSECOMContactPerson;
                    ViewBag.SiteCode = dsSiteInfo.dtSiteData[0].SiteCode_Short;
                    ViewBag.SiteNameEN = dsSiteInfo.dtSiteData[0].SiteNameEN;
                    ViewBag.SiteAddrEN = dsSiteInfo.dtSiteData[0].AddressFullEN;
                    ViewBag.SiteNameLC = dsSiteInfo.dtSiteData[0].SiteNameLC;
                    ViewBag.SiteAddrLC = dsSiteInfo.dtSiteData[0].AddressFullLC;
                    ViewBag.PersonInCharge = dsSiteInfo.dtSiteData[0].PersonInCharge;
                    ViewBag.SECOMContactPerson = dsSiteInfo.dtSiteData[0].SECOMContactPerson;

                    // New requirement 27/Feb/2012
                    ViewBag.PhoneNo = dsSiteInfo.dtSiteData[0].PhoneNo;
                    ViewBag.BuildingUsage = dsSiteInfo.dtSiteData[0].BuildingUsageCodeName;

                    ViewBag.txtAttachImportanceFlag = dsSiteInfo.dtSiteData[0].SpecialCareFlag; 
                }
            }

            return View();


        }

        /// <summary>
        /// Get config for Contract List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS280()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS280", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Transform contract list in ScreenParameter to xml format and show to screen.
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public ActionResult CMS280_GetContract(string siteCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS280_ScreenParameter param = GetScreenObject<CMS280_ScreenParameter>();
                if (param.dsSiteIfoForView == null)
                    param.dsSiteIfoForView = new dsSiteInfoForView();
                if (param.dsSiteIfoForView.dtContractsSameSite == null)
                    param.dsSiteIfoForView.dtContractsSameSite = new List<dtContractsSameSite>();
                dsSiteInfoForView dsSiteInfo = param.dsSiteIfoForView;

                CommonUtil CommU = new CommonUtil();
                //dsSiteInfo.dtSiteData[0].SiteCode = CommU.ConvertSiteCode(dsSiteInfo.dtSiteData[0].SiteCode,
                //    CommonUtil.CONVERT_TYPE.TO_SHORT);

                //for (int i = 0; i < dsSiteInfo.dtContractsSameSite.Count; i++)
                //    dsSiteInfo.dtContractsSameSite[i].ContractCode = CommU.ConvertContractCode(dsSiteInfo.dtContractsSameSite[i].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);


                List<View_dtContractSameSite> ViewContractSameSite = CommonUtil.ConvertObjectbyLanguage<dtContractsSameSite, View_dtContractSameSite>(dsSiteInfo.dtContractsSameSite, "ProductName", "LastChangeTypeName");

                res.ResultData = CommonUtil.ConvertToXml<View_dtContractSameSite>(ViewContractSameSite, "Common\\CMS280", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Get Site Information and Contract with same site.
        /// </summary>
        /// <param name="strSiteCode"></param>
        /// <returns></returns>
        public dsSiteInfoForView GetSiteInfoForView(string strSiteCode)
        {
            try
            {
                dsSiteInfoForView SiteInfoForView = new dsSiteInfoForView();
                ISiteMasterHandler handSite = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                doSiteSearchCondition doSiteSeachCond = new doSiteSearchCondition();
                doSiteSeachCond.SiteCode = strSiteCode;
                List<dtSiteData> dtSiteData = handSite.GetSiteDataForSearch(doSiteSeachCond);

                CommonUtil.MappingObjectLanguage<dtSiteData>(dtSiteData);


                if (dtSiteData.Count <= 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                }
                else
                {
                    SiteInfoForView.dtSiteData = dtSiteData;
                }
                   
                IViewContractHandler handView = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtContractsSameSite> dtContSameSite = handView.GetContractsListForViewSite(strSiteCode);

                SiteInfoForView.dtContractsSameSite = dtContSameSite;
                return SiteInfoForView;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
