//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

using SECOM_AJIS.DataEntity.Common;
using System.Collections.Generic;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS260
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS260_Authority(CMS260_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
           

            return InitialScreenEnvironment<CMS260_ScreenParameter>("CMS260", param, res);
        }

        /// <summary>
        ///  Method for return view of screen CMS260
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS260")]
        public ActionResult CMS260()
        {
            string strRealCustomerCode = "";

            try
            {
                CMS260_ScreenParameter param = GetScreenObject<CMS260_ScreenParameter>();
                strRealCustomerCode = param.strRealCustomerCode;
            }
            catch
            {
            }

            CommonUtil c = new CommonUtil();

            // convert strRealCustomerCode to long format
            strRealCustomerCode = c.ConvertCustCode(strRealCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            List<View_dtCustomerForView> nlst = new List<View_dtCustomerForView>();

            try
            {


                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                List<dtCustomerForView> list = handler.GetCustomerForView(strRealCustomerCode, MiscType.C_CUST_STATUS, MiscType.C_CUST_TYPE, MiscType.C_FINANCIAL_MARKET_TYPE);

                foreach (dtCustomerForView l in list)
                {
                    nlst.Add(CommonUtil.CloneObject<dtCustomerForView, View_dtCustomerForView>(l));
                }

                // select language
                nlst = CommonUtil.ConvertObjectbyLanguage<View_dtCustomerForView, View_dtCustomerForView>(nlst, "CustStatusName");


                if (nlst.Count > 0)
                {
                    ViewBag.CustomerStatus = CommonUtil.TextCodeName(nlst[0].CustStatus, nlst[0].CustStatusName);
                    ViewBag.CustomerCode = c.ConvertCustCode(nlst[0].CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    //ViewBag.CustomerName_Eng = nlst[0].CustNameEN;
                    //ViewBag.CustomerName_Local = nlst[0].CustNameLC;
                    ViewBag.CustomerName_Eng = nlst[0].CustFullNameEN;
                    ViewBag.CustomerName_Local = nlst[0].CustFullNameLC;
                }




                return View();
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Initial grid of screen CMS260
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS260_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS260"));
        }

        /// <summary>
        /// Get site data by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS260_SearchResponse(doSiteSearchCondition cond)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<View_dtSiteData> nlst = new List<View_dtSiteData>();

            // Create string Customer status with commar separate. like ,xx,yy,zz,           
            List<string> lstCustStatus = new List<string>();
            lstCustStatus.Add(cond.chkExistingCustomer);
            lstCustStatus.Add(cond.chkNewCustomer);
            cond.CustStatus = CommonUtil.CreateCSVString(lstCustStatus);

            CommonUtil c = new CommonUtil();

            


            try
            {
                // Convert search condition to long format
                cond.CustomerCode = c.ConvertCustCode(cond.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                cond.SiteCode = c.ConvertSiteCode(cond.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                string[] strs = { "Counter" };

                if (CommonUtil.IsNullAllField(cond ,strs))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    

                    // check require field
                    if (cond.isSearchByRealCust)
                    {
                        if (CommonUtil.IsNullOrEmpty(cond.CustStatus) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0054);
                        }
                    }

                    //if (cond.Counter == 0)
                    //{
                    //    res.ResultData = CommonUtil.ConvertToXml<View_dtSiteData>(nlst, "Common\\CMS260", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    //    return Json(res);
                    //}

                    ISiteMasterHandler handler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                    List<dtSiteData> list = handler.GetSiteDataForSearch(cond);


                    foreach (dtSiteData l in list)
                    {
                        nlst.Add(CommonUtil.CloneObject<dtSiteData, View_dtSiteData>(l));
                    }
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                nlst = new List<View_dtSiteData>();
                res.AddErrorMessage(ex);

            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtSiteData>(nlst, "Common\\CMS260", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }
    }
}
