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
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using SECOM_AJIS.Common.Models;
using CrystalDecisions.CrystalReports.Engine;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check suspend, authority and resume of BLS032
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS032_Authority(BLS032_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
             
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<BLS032_ScreenParameter>("BLS032", param, res);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initial screen BLS032 and set view bag of credit card data
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS032")]
        public ActionResult BLS032()
        {
            string[] strCreditnoArr = null;
            BLS032_ScreenParameter param = GetScreenObject<BLS032_ScreenParameter>();
            if (param.doCredit != null)
            {
                ViewBag.ContractCode = param.doCredit.BillingCode_Short;
                ViewBag.BillingClientCode = param.doCredit.BillingClientCode;
                ViewBag.BillingClientNameEN = param.doCredit.BillingClientNameEN;
                ViewBag.BillingClientNameLC = param.doCredit.BillingClientNameLC;

                ViewBag.CreditCardType = param.doCredit.CreditCardType;
                ViewBag.CardName = param.doCredit.CardName;
                ViewBag.CreditCardCompanyCode = param.doCredit.CreditCardCompanyCode;
                if (!CommonUtil.IsNullOrEmpty(param.doCredit.CreditCardNo))
                {
                    strCreditnoArr = param.doCredit.CreditCardNo.Split('-');
                    if (strCreditnoArr.Length == 4)
                    {                        
                        ViewBag.CreditCardNo1 = strCreditnoArr[0];
                        ViewBag.CreditCardNo2 = strCreditnoArr[1];
                        ViewBag.CreditCardNo3 = strCreditnoArr[2];
                        ViewBag.CreditCardNo4 = strCreditnoArr[3];
                    }                
                }              
                ViewBag.ExpMonth = param.doCredit.ExpMonth;
                ViewBag.ExpYear = param.doCredit.ExpYear;
            }

            return View();
        }

        #endregion                              

        #region Action Result
          /// <summary>
        /// Confirm or OK for credit card data
          /// </summary>
          /// <param name="valid"></param>
          /// <param name="doCredit"></param>
          /// <returns></returns>
        public ActionResult BLS032_ConfirmData(BLS032_ScreenInputValidate valid,doCreditCard doCredit)
        {
            ObjectResultData res = new ObjectResultData();
            BLS032_ScreenParameter param = GetScreenObject<BLS032_ScreenParameter>();
            try
            {
                if (ModelState.IsValid == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                ValidatorUtil validator = new ValidatorUtil();

                if (string.IsNullOrEmpty(valid.CreditCardNo1) == false)
                {
                    if (valid.CreditCardNo1.Length < 4)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS032",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        "BLS032_CreditCardNo1",
                                        "lblCreditCardNo",
                                        "BLS032_CreditCardNo1");
                    }
                }

                if (string.IsNullOrEmpty(valid.CreditCardNo2) == false)
                {
                    if (valid.CreditCardNo2.Length < 4)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                       "BLS032",
                                       MessageUtil.MODULE_COMMON,
                                       MessageUtil.MessageList.MSG0007,
                                       "BLS032_CreditCardNo2",
                                       "lblCreditCardNo",
                                       "BLS032_CreditCardNo2");
                    }
                }

                if (string.IsNullOrEmpty(valid.CreditCardNo3) == false)
                {
                    if (valid.CreditCardNo3.Length < 4)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                       "BLS032",
                                       MessageUtil.MODULE_COMMON,
                                       MessageUtil.MessageList.MSG0007,
                                       "BLS032_CreditCardNo3",
                                       "lblCreditCardNo",
                                       "BLS032_CreditCardNo3");
                    }
                }

                if (string.IsNullOrEmpty(valid.CreditCardNo4) == false)
                {
                    if (valid.CreditCardNo4.Length < 4)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                       "BLS032",
                                       MessageUtil.MODULE_COMMON,
                                       MessageUtil.MessageList.MSG0007,
                                       "BLS032_CreditCardNo4",
                                       "lblCreditCardNo",
                                       "BLS032_CreditCardNo4");
                    }
                }


                if (valid.ExpYear.Length < 4)
                {

                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS032",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BLS032_ExpYear",
                                         "lblExpireDateYear",
                                         "BLS032_ExpYear");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
               

                if (res.IsError)
                {
                    res.MessageType = SECOM_AJIS.Common.Models.MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }



                res.ResultData = doCredit;

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
