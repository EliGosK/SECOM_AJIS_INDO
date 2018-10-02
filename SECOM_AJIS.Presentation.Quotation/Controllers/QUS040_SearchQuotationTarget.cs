using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Presentation.Quotation.Models;

namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    public partial class QuotationController : BaseController
    {
        private const string QUS040_Screen = "QUS040";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult QUS040_Authority(QUS040_ScreenParameter Cond)
        {
            return InitialScreenEnvironment<QUS040_ScreenParameter>(QUS040_Screen,Cond);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(QUS040_Screen)]
        public ActionResult QUS040()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// Initial grid control
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_QUS040()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.ResultData = CommonUtil.ConvertToXml<View_dtSearchQuotationTargetListlResult>(null, "Quotation\\QUS040", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load result data to grid
        /// </summary>
        /// <param name="doCond"></param>
        /// <returns></returns>
        public ActionResult QUS040_XML(QUS040_SearchQuotationTarget doCond)
        {
            string XMLpath = "Quotation\\QUS040";
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                List<View_dtSearchQuotationTargetListlResult> nlst = null;

                ValidatorUtil.BuildErrorMessage(res, this, new object[] { doCond });
                if (res.IsError == false)
                {
                    CommonUtil ComUtil = new CommonUtil();
                    doCond.QuotationTargetCode = ComUtil.ConvertQuotationTargetCode(doCond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    doCond.ContractTargetCode = ComUtil.ConvertCustCode(doCond.ContractTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    doCond.SiteCode = ComUtil.ConvertSiteCode(doCond.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                    IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    List<dtSearchQuotationTargetListResult> lst = handler.SearchQuotationTargetList(doCond);

                    nlst = CommonUtil.ConvertObjectbyLanguage<dtSearchQuotationTargetListResult, View_dtSearchQuotationTargetListlResult>(lst, "QuotationOfficeName", "OperationOfficeName", "EmpFullName");
                }
                if (doCond.QuotationDateFrom != null && doCond.QuotationDateTo != null)
                {
                    if (DateTime.Compare(Convert.ToDateTime(doCond.QuotationDateFrom), Convert.ToDateTime(doCond.QuotationDateTo)) > 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2001, null, new string[] { "QuotationDateFrom", "QuotationDateTo" });
                        return Json(res);
                    }
                }

                res.ResultData = CommonUtil.ConvertToXml<View_dtSearchQuotationTargetListlResult>(nlst, XMLpath, CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}

