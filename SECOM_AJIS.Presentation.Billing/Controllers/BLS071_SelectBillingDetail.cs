using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Presentation.Billing.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;

using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.Presentation.Common.Helpers;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check suspend, authority and resume of BLS071
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS071_Authority(BLS071_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<BLS071_ScreenParameter>("BLS071", param, res);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initial screen BLS071 and set view bag of billing target data
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS071")]
        public ActionResult BLS071()
        {
            BLS071_ScreenParameter param = GetScreenObject<BLS071_ScreenParameter>();
            if (param.doBillingTarget != null)
            {

                ViewBag.BillingTargetCode = param.doBillingTarget.BillingTargetCodeShort;
                ViewBag.FullNameEN = param.doBillingTarget.FullNameEN;
                ViewBag.FullNameLC = param.doBillingTarget.FullNameLC;
                ViewBag.currency = param.currency;
            }

            return View();
        }

        #endregion

        #region Action Result
        /// <summary>
        /// Get billing detail for combine
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS071_GetBillingDetailForCombine()
        {
            ObjectResultData res = new ObjectResultData();
            BLS071_ScreenParameter param = GetScreenObject<BLS071_ScreenParameter>();
            try
            {

                List<BLS071_BillingDetail> doBillingDetail = new List<BLS071_BillingDetail>();
                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                CommonUtil cm = new CommonUtil();

                if (param.dtOldBillingDetailList.Count > 0)
                {
                    string strBillingTargetCode_long = cm.ConvertBillingTargetCode(param.doBillingTarget.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    string strBillingTypeCode = param.dtOldBillingDetailList[0].BillingTypeCode;

                    List<doBillingDetail> doBillingDetailForCombineList = handler.GetBillingDetailForCombine(strBillingTargetCode_long, strBillingTypeCode,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US, param.currencyCode);
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    lst = hand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                    foreach(var d in doBillingDetailForCombineList)
                    {
                        if (string.IsNullOrEmpty(d.BillingAmountCurrencyType)) d.BillingAmountCurrencyType = "1";
                        if (d.BillingAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US) d.BillingAmount = d.BillingAmountUsd;
                        if (d.BillingAmount == null) d.BillingAmount = 0;
                        d.BillingAmountCurrencyTypeName = lst.Where(m => m.ValueCode == d.BillingAmountCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();
                    }


                    doBillingDetail = CommonUtil.ClonsObjectList<doBillingDetail, BLS071_BillingDetail>(doBillingDetailForCombineList);

                    

                }

                param.doBillingDetailForCombineList = doBillingDetail;

                res.ResultData = CommonUtil.ConvertToXml<BLS071_BillingDetail>(doBillingDetail, "Billing\\BLS071_BillingDetail", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Confirm or OK for select billing detail
        /// </summary>
        /// <param name="doSelectCheckBillingDetail"></param>
        /// <returns></returns>
        public ActionResult BLS071_ConfirmData(List<BLS071_TempBillingForFilter> doSelectCheckBillingDetail)
        {
            ObjectResultData res = new ObjectResultData();
            BLS071_ScreenParameter param = GetScreenObject<BLS071_ScreenParameter>();                    
            try
            {
                List<string> lstBillingCode = new List<string>();
                List<int> lstBillingDetailNo = new List<int>();
                if (doSelectCheckBillingDetail != null && doSelectCheckBillingDetail.Count > 0)
                {
                    var keyList = from p in doSelectCheckBillingDetail select p.key;

                    List<BLS071_BillingDetail> selectBillingDetail = (from p in param.doBillingDetailForCombineList
                                                                      where keyList.Contains( p.BillingCode+ Convert.ToString(p.BillingDetailNo) )   
                                                                      select p).ToList<BLS071_BillingDetail>();

                    res.ResultData = selectBillingDetail;
                }
                
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        #endregion
    }
}
