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
using System.Globalization;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check suspend, authority and resume of BLS031
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS031_Authority(BLS031_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ////// Test handler //////////////               
                //IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ////List<doBillingTempBasic> lst = new List<doBillingTempBasic>();
                ////lst.Add(new doBillingTempBasic
                ////{
                ////    ContractCode = "N0000000099",
                ////    BillingOCC = "01",
                ////    BillingTargetCode = "0000000004-001",
                ////    BillingClientCode = "0000000148",
                ////    BillingOfficeCode = "0001"
                ////});
                //handler.ManageBillingBasicForStart("N0000000099", Convert.ToDateTime("15-Dec-2011"), Convert.ToDateTime("05-Mar-2012"));
                ////////////////////////////////

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }



            return InitialScreenEnvironment<BLS031_ScreenParameter>("BLS031", param, res);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initial screen BLS031 and set view bag autotransfer data
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS031")]
        public ActionResult BLS031()
        {
            string[] strAccountNoArray = null;
            BLS031_ScreenParameter param = GetScreenObject<BLS031_ScreenParameter>();
            if (param.doAutoTransfer != null)
            {
                ViewBag.ContractCode = param.doAutoTransfer.BillingCode_Short;
                ViewBag.BillingClientCode = param.doAutoTransfer.BillingClientCode;
                ViewBag.BillingClientNameEN = param.doAutoTransfer.BillingClientNameEN;
                ViewBag.BillingClientNameLC = param.doAutoTransfer.BillingClientNameLC;
                ViewBag.AccountName = param.doAutoTransfer.AccountName;
                ViewBag.BankCode = param.doAutoTransfer.BankCode;
                ViewBag.BankBranchCode = param.doAutoTransfer.BankBranchCode;

                ViewBag.Account1 = param.doAutoTransfer.AccountNo;

                // Commeny by Jirawat Jannet: 2016-08-22
                //if (!CommonUtil.IsNullOrEmpty(param.doAutoTransfer.AccountNo))
                //{
                //    strAccountNoArray = param.doAutoTransfer.AccountNo.Split('-');
                //    if (strAccountNoArray.Length == 4)
                //    {
                //        ViewBag.Account1 = strAccountNoArray[0];
                //        ViewBag.Account2 = strAccountNoArray[1];
                //        ViewBag.Account3 = strAccountNoArray[2];
                //        ViewBag.Account4 = strAccountNoArray[3];
                //    }
                //}
                ViewBag.AccountType = param.doAutoTransfer.AccountType;
                ViewBag.LastestResult = param.doAutoTransfer.LastestResult;
                ViewBag.AutoTransferDate = param.doAutoTransfer.AutoTransferDate;

                // Add by Jirawat Jannet 2016-09-02
                IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_BankBranch> list = hand.GetTbm_BankBranch("0000");
                if (list != null && list.Count > 0)
                    ViewBag.BankBranchCode = list[0].BankBranchCode;
                else
                    ViewBag.BankBranchCode = "0000";
                // End add
            }

            return View();
        }

        #endregion

        #region Action Result
        /// <summary>
        /// Get bank branch data
        /// </summary>
        /// <param name="BankCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BLS031_BankBranchData(string BankCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_BankBranch> list = hand.GetTbm_BankBranch(BankCode);

                //var sortedList = from p in lst
                //                 orderby p.BankBranchName
                //                 select p;

                //lst = sortedList.ToList<tbm_BankBranch>();

                CultureInfo culture = null;
                string strDisplayName = "BankBranchNameEN";

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN
                    || CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "BankBranchNameEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.BankBranchNameEN, StringComparer.Create(culture, false)).ToList();
                }
                else
                {
                    strDisplayName = "BankBranchNameLC";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.BankBranchNameLC, StringComparer.Create(culture, false)).ToList();
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_BankBranch>(list, strDisplayName, "BankBranchCode", false);
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Confirm or OK for auto transfer data
        /// </summary>
        /// <param name="valid"></param>
        /// <param name="doAutoTransfer"></param>
        /// <returns></returns>
        public ActionResult BLS031_ConfirmData(BLS031_ScreenInputValidate valid, AutoTransfer doAutoTransfer)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            BLS031_ScreenParameter param = GetScreenObject<BLS031_ScreenParameter>();
           
            try
            {
                if (ModelState.IsValid == false)
                {  
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                List<string> controlsList = new List<string>();

                if (string.IsNullOrEmpty(valid.Account1) == false)
                {
                    if (valid.Account1.Length < 3)
                    {
                        controlsList.Add("BLS031_Account1");
                    }
                }

                //if (string.IsNullOrEmpty(valid.Account2) == false)
                //{
                //    if (valid.Account2.Length < 1)
                //    {
                //        controlsList.Add("BLS031_Account2");
                //    }
                //}

                //if (string.IsNullOrEmpty(valid.Account3) == false)
                //{
                //    if (valid.Account3.Length < 5)
                //    {
                //        controlsList.Add("BLS031_Account3");
                //    }
                //}

                //if (string.IsNullOrEmpty(valid.Account4) == false)
                //{
                //    if (valid.Account4.Length < 1)
                //    {
                //        controlsList.Add("BLS031_Account4");
                //    }
                //}

                if (controlsList.Count > 0)
                {
                   string strLebelAccount = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_BILLING, "BLS031", "lblAccountNo");
                   res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { strLebelAccount }, controlsList.ToArray());
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                res.ResultData = doAutoTransfer;

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        #endregion
    }
}
