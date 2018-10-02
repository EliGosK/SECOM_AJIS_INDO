using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Billing;

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
        /// <summary>
        /// Check suspend, authority and resume of CMS470
        /// </summary>
        /// <param name="param">Screen parameter</param>
        /// <returns></returns>
        public ActionResult CMS470_Authority(CMS470_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
             
                //Check permission
                //if (CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_BILLING_INFORMATION, FunctionID.C_FUNC_ID_VIEW) == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}                                             
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS470_ScreenParameter>("CMS470", param, res);
        }

        /// <summary>
        /// Initialize screen CMS470
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS470")]
        public ActionResult CMS470()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil cm = new CommonUtil();
            try
            {
                CMS470_ScreenParameter param = GetScreenObject<CMS470_ScreenParameter>();

                // Prepare for show section                   
                ViewBag.txtCallerScreenId = param.CallerScreenID;
              
                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial Billing Target Grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS470_InitialBillingTargetGrid()
        {
            return Json(CommonUtil.ConvertToXml<View_dtBillingTargetData>(null, "Common\\CMS470_ResultBillingTarget", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search and get view billing target data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS470_SearchResponse(doBillingTargetDataSearchCondition cond)
        { 
            ObjectResultData res = new ObjectResultData();
            CommonUtil c = new CommonUtil();
            List<View_dtBillingTargetData> nlst = new List<View_dtBillingTargetData>();
            List<dtBillingTargetData> list = new List<dtBillingTargetData>();
            try
            {

                /// Validate Require ////////////////
                if (CommonUtil.IsNullOrEmpty(cond.CMS470_chkJuristic) && CommonUtil.IsNullOrEmpty(cond.CMS470_chkIndividual)
                    && CommonUtil.IsNullOrEmpty(cond.CMS470_chkAssociation) && CommonUtil.IsNullOrEmpty(cond.CMS470_chkPublicOffice)
                    && CommonUtil.IsNullOrEmpty(cond.CMS470_chkOther))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                       ScreenID.C_SCREEN_ID_SEARCH_BILLING_INFORMATION,
                                       MessageUtil.MODULE_COMMON,
                                       MessageUtil.MessageList.MSG0055,null,null);
                    return Json(res);                 
                }
             
                   
                
                // Concate string CustomerTypeCode with commar separate. like ,xx,yy,zz, 
                List<string> lstCustomerTypeCode = new List<string>();
                lstCustomerTypeCode.Add(cond.CMS470_chkJuristic);
                lstCustomerTypeCode.Add(cond.CMS470_chkIndividual);
                lstCustomerTypeCode.Add(cond.CMS470_chkAssociation);
                lstCustomerTypeCode.Add(cond.CMS470_chkPublicOffice);
                lstCustomerTypeCode.Add(cond.CMS470_chkOther);
                cond.CMS470_CustomerTypeCode = CommonUtil.CreateCSVString(lstCustomerTypeCode);
                cond.CMS470_BillingClientCode = c.ConvertBillingClientCode(cond.CMS470_BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                if (cond.CMS470_CustomerTypeCode == string.Empty)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0055);
                    //return Json(res);
                }
                else
                {
                   
                    IViewBillingHandler handler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                    list = handler.GetViewBillingTargetDataForSearch(cond);

                    foreach (dtBillingTargetData l in list)
                    {
                        nlst.Add(CommonUtil.CloneObject<dtBillingTargetData, View_dtBillingTargetData>(l));
                    }
                }

            }
            catch (Exception ex)
            {
                //nlst = new List<View_dtBillingClientData>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);

            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtBillingTargetData>(nlst, "Common\\CMS470_ResultBillingTarget", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);

        }

        /// <summary>
        /// Get billing target for view
        /// </summary>
        /// <param name="BillingTargetCode">Billing target code</param>
        /// <returns></returns>
        public ActionResult CMS470_GetBillingTargetForView(string BillingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil cm = new CommonUtil();
            try
            {
                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler; 
                string strBillingTargetCode = cm.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                List<dtTbt_BillingTargetForView> lst =  handler.GetTbt_BillingTargetForView(strBillingTargetCode, MiscType.C_CUST_TYPE);
                res.ResultData = lst; 
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
               
            }
            return Json(res);
        }

    }
}
