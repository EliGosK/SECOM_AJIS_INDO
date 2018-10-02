using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Presentation.Quotation.Models;


namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    public partial class QuotationController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult QUS010_Authority(QUS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (param.IsPopup)
                {
                    if (param.CallerScreenID == ScreenID.C_SCREEN_ID_FN99
                        || param.CallerScreenID == ScreenID.C_SCREEN_ID_FQ99)
                    {
                        doQUS010Condition_FN_Q99 doFNQ99 = CommonUtil.CloneObject<QUS010_ScreenParameter, doQUS010Condition_FN_Q99>(param);
                        ValidatorUtil.BuildErrorMessage(res, this, new object[] { doFNQ99 });
                        if (res.IsError)
                            return Json(res);
                    }
                    else if (param.CallerScreenID == ScreenID.C_SCREEN_ID_CP12_MODIFY_INSTRUMENT_QTY
                        || param.CallerScreenID == ScreenID.C_SCREEN_ID_CP12_CHANGE_PLAN
                        || param.CallerScreenID == ScreenID.C_SCREEN_ID_CQ12_CHANGE_PLAN)
                    {
                        doQUS010Condition_CPQ12 doCPQ99 = CommonUtil.CloneObject<QUS010_ScreenParameter, doQUS010Condition_CPQ12>(param);
                        ValidatorUtil.BuildErrorMessage(res, this, new object[] { doCPQ99 });
                        if (res.IsError)
                            return Json(res);
                    }
                }
                else if (!CheckUserPermission("QUS010", FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<QUS010_ScreenParameter>("QUS010", param);
        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("QUS010")]
        public ActionResult QUS010()
        {


            QUS010_ScreenParameter Cond = GetScreenObject<QUS010_ScreenParameter>();

            if (!Cond.IsPopup)
                ViewBag.ViewMode = "1";
            else
                ViewBag.ViewMode = "2";

            IQuotationHandler hand = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            ViewBag.strCallerScreenID = Cond.CallerScreenID;
            ViewBag.strServiceTypeCode = Cond.strServiceTypeCode;
            ViewBag.strTargetCodeTypeCode = Cond.strTargetCodeTypeCode;
            ViewBag.strQuotationTargetCode = Cond.strQuotationTargetCode;
            ViewBag.C_SCREEN_ID_MAIN = ScreenID.C_SCREEN_ID_MAIN;
            ViewBag.C_SCREEN_ID_FN99 = ScreenID.C_SCREEN_ID_FN99;
            ViewBag.C_SCREEN_ID_FQ99 = ScreenID.C_SCREEN_ID_FQ99;
            ViewBag.C_SCREEN_ID_FN99 = ScreenID.C_SCREEN_ID_FN99;
            ViewBag.C_SCREEN_ID_FQ99 = ScreenID.C_SCREEN_ID_FQ99;
            ViewBag.C_LOCK_STATUS_UNLOCK = LockStatus.C_LOCK_STATUS_UNLOCK;
            ViewBag.C_SCREEN_ID_MAIN = ScreenID.C_SCREEN_ID_MAIN;
            ViewBag.C_SCREEN_ID_CP12_PLAN = ScreenID.C_SCREEN_ID_CP12_CHANGE_PLAN;
            ViewBag.C_SCREEN_ID_CP12_INST = ScreenID.C_SCREEN_ID_CP12_MODIFY_INSTRUMENT_QTY;
            ViewBag.C_SCREEN_ID_CQ12 = ScreenID.C_SCREEN_ID_CQ12_CHANGE_PLAN;
            ViewBag.C_PROD_TYPE_SALE = ProductType.C_PROD_TYPE_SALE;

            return View();
        }
        /// <summary>
        /// Initial grid in case of open from screen CTS010, CTS020
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_QUS010()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Quotation\\QUS010"));
        }
        /// <summary>
        /// Initial grid in case of open from all screen except CTS010, CTS020, CTS051, CTS052, CTS062
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_QUS010_nonSel()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Quotation\\QUS010_nonSel"));
        }
        /// <summary>
        /// Initial grid in case of open from screen CTS051, CTS052, CTS062
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QUS010_XML(QUS010_SearchQuotation Cond)
        {
            QUS010_ScreenParameter param = GetScreenObject<QUS010_ScreenParameter>();

            string xmlPath = "Quotation\\QUS010";
            if (param.IsPopup == false) // plain screen
                xmlPath = "Quotation\\QUS010_nonSel";



            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<View_dtSearchQuotationListResult> dtSearchQuo = new List<View_dtSearchQuotationListResult>();

            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { Cond });
                if (res.IsError)
                {
                    res.ResultData = CommonUtil.ConvertToXml<View_dtSearchQuotationListResult>(dtSearchQuo, xmlPath, CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    return Json(res);
                }
                if (Cond.QuotationDateFrom != null && Cond.QuotationDateTo != null)
                {
                    if (DateTime.Compare(Convert.ToDateTime(Cond.QuotationDateFrom), Convert.ToDateTime(Cond.QuotationDateTo)) > 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2001, null, new string[] { "QuotationDateFrom", "QuotationDateTo" });
                        return Json(res);
                    }
                }
                CommonUtil ComU = new CommonUtil();
                Cond.QuotationTargetCode = ComU.ConvertQuotationTargetCode(Cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                Cond.ContractTargetCode = ComU.ConvertCustCode(Cond.ContractTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                Cond.SiteCode = ComU.ConvertSiteCode(Cond.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                if (!(param.CallerScreenID == ScreenID.C_SCREEN_ID_FN99 || param.CallerScreenID == ScreenID.C_SCREEN_ID_FQ99))
                {
                    Cond.ServiceTypeCode = null;
                    Cond.TargetCodeTypeCode = null;
                }
                Cond.ContractTransferStatus = param.strContractTransferStatus;

                IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                List<dtSearchQuotationListResult> lst = handler.SearchQuotationList(Cond);
                dtSearchQuo = CommonUtil.ClonsObjectList<dtSearchQuotationListResult, View_dtSearchQuotationListResult>(lst);

                CommonUtil.MappingObjectLanguage<View_dtSearchQuotationListResult>(dtSearchQuo);

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                MiscTypeMappingList lstMiscMap = new MiscTypeMappingList();
                lstMiscMap.AddMiscType(dtSearchQuo.ToArray());
                hand.MiscTypeMappingList(lstMiscMap);

                res.ResultData = CommonUtil.ConvertToXml<View_dtSearchQuotationListResult>(dtSearchQuo, xmlPath, CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            
            return Json(res);
        }
    }
}


