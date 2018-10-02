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

using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS422
        /// </summary>
        /// <param name="param">Screen parameter</param>
        /// <returns></returns>
        public ActionResult CMS422_Authority(CMS422_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //Check permission
                //if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CREDIT_CARD_INFORMATION, FunctionID.C_FUNC_ID_VIEW) == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                // is parameter OK ?
                if (CommonUtil.IsNullOrEmpty(param.ContractCode) && CommonUtil.IsNullOrEmpty(param.BillingOCC))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }

                // check data exist
                CommonUtil cm = new CommonUtil();
                string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strBillingOCC = param.BillingOCC;
                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<dtTbt_CreditCardForView> listCreditCardForView = handler.GetTbt_CreditCardForView(strContractCode, strBillingOCC);
                if (listCreditCardForView.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }
                return InitialScreenEnvironment<CMS422_ScreenParameter>("CMS422", param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }


        [Initialize("CMS422")]
        /// <summary>
        /// Initialize screen of CMS422
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS422()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS422_ScreenParameter param = GetScreenObject<CMS422_ScreenParameter>();
                CommonUtil cm = new CommonUtil();


                string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strBillingOCC = param.BillingOCC;

                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<dtTbt_CreditCardForView> listCreditCardForView = handler.GetTbt_CreditCardForView(strContractCode, strBillingOCC);

                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (listCreditCardForView.Count > 0)
                {
                    ViewBag.txtBillingCode = listCreditCardForView[0].BillingCode_short;
                    ViewBag.txtBillingClientCode = listCreditCardForView[0].BillingClientCode_Short;
                    ViewBag.txtBillingClientNameLC = listCreditCardForView[0].FullNameLC;
                    ViewBag.txtBillingClientNameEN = listCreditCardForView[0].FullNameEN;
                    ViewBag.txtCreditCardType = listCreditCardForView[0].CreditCardTypeName;
                    ViewBag.txtCardName = listCreditCardForView[0].CardName;
                    ViewBag.txtCreditCardCompany = listCreditCardForView[0].CreditCardCompanyName;
                    ViewBag.txtCreditCardNo = listCreditCardForView[0].CreditCardNo_ForView; // listCreditCardForView[0].CreditCardNo; // Edit by Narupon W. 28/08/2012
                    ViewBag.txtExpireDate = listCreditCardForView[0].ExpireDate;
                    //ViewBag.txtTransferDate = CommonUtil.TextDate(listCreditCardForView[0].AutoTransferDate);                
                    ViewBag.txtTransferDate = listCreditCardForView[0].AutoTransferDateForView;
                }

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }


    }
}
