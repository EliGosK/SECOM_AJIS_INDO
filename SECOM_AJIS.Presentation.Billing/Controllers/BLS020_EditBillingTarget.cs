using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Transactions;

using CSI.WindsorHelper;

using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Presentation.Billing.Models;

using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check suspend, authority and resume of BLS020 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS020_Authority(BLS020_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ///////// Check Suspending //////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                ///// Check permission //////////
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_TARGET, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<BLS020_ScreenParameter>("BLS020", param, res);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initial screen BLS020
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS020")]
        public ActionResult BLS020()
        {
            BLS020_ScreenParameter param = GetScreenObject<BLS020_ScreenParameter>();
            if (param != null)
            {
                // param.BillingTargetCode = "0000000369-001";
                if (!CommonUtil.IsNullOrEmpty(param.BillingTargetCode))
                {
                    string[] str = null;
                    str = param.BillingTargetCode.Split('-');
                    if (str.Length == 2)
                    {
                        ViewBag.BillingClientCode = str[0];
                        ViewBag.BillingTargetNo = str[1];
                        ViewBag.BillingTargetCode = param.BillingTargetCode;
                    }

                }
            }
            ViewBag.BillingOfficeCboValue = BillingOfficeWithSpecialConditionBLS020();

            return View();
        }

        #endregion

        #region Method
        /// <summary>
        /// Get billing office with special condition
        /// </summary>
        /// <returns></returns>
        public string BillingOfficeWithSpecialConditionBLS020()
        {
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                // Get all office data
                list = handler.GetTbm_Office();
                List<tbm_Office> headOffice = (from p in list
                                               where p.OfficeCode == CommonUtil.dsTransData.dtUserData.MainOfficeCode
                                               select p).ToList<tbm_Office>();

                if (headOffice.Count > 0)
                {
                    //headOffice[0].OfficeLevel == 
                    // Comment by : Jirawat Jannet: 2016-08-17
                    //if (headOffice[0].OfficeLevel == InventoryHeadOffice.C_OFFICELEVEL_HEAD) // if yes --> Authen cbo
                    //{
                    //    // Filter ==> BillingOfficeNormalCbo where ==> FunctionBilling <> C_FUNC_BILLING_NO
                    //    //list = (from p in list where p.FunctionBilling != FunctionBilling.C_FUNC_BILLING_NO select p).ToList<tbm_Office>();
                    //    return string.Empty;
                    //}
                    //else
                    //{
                    //    // list = headOffice;
                    //    return headOffice[0].OfficeCode;
                    //}
                    return headOffice[0].OfficeCode;
                }
                else
                {
                    list = new List<tbm_Office>();
                    return string.Empty;
                }

                // Language mappping
                //CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
                return string.Empty;
            }
        }

        /// <summary>
        /// Check existing billing office
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <returns></returns>
        public bool BLS020_ValidExistOffice(string OfficeCode)
        {
            if (CommonUtil.IsNullOrEmpty(OfficeCode) == false)
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                // Get all office data
                List<tbm_Office>  list = handler.GetTbm_Office();
                var lstheadOffice = (from p in list
                                     where p.OfficeLevel == InventoryHeadOffice.C_OFFICELEVEL_HEAD
                                     select p.OfficeCode).ToList<string>();


                var bHasHeadOffice = (from p in CommonUtil.dsTransData.dtOfficeData
                                      where lstheadOffice.Contains(p.OfficeCode)
                                      select p
                                     ).Count() > 0;

                var existsBillingOffice = CommonUtil.dsTransData.dtUserBelongingData.Where(x => x.OfficeCode == OfficeCode);
                
                if (!bHasHeadOffice && existsBillingOffice.Count() <= 0)
                {
                    return false;
                }
                else // Head Office can to access all billing target
                {
                    return true;
                }
            }
            return false;

        }
        #endregion

        #region Event
        /// <summary>
        /// Retrieve billing target data
        /// </summary>
        /// <param name="BillingClientCode"></param>
        /// <param name="BillingTargetNo"></param>
        /// <param name="BillingTargetCode"></param>
        /// <returns></returns>
        public ActionResult BLS020_RetrieveBillingTargetData(string BillingClientCode, string BillingTargetNo, string BillingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            BLS020_ScreenParameter sParam = GetScreenObject<BLS020_ScreenParameter>();
            try
            {
                //Check mandatory
                List<string> lstControl = new List<string>();
                if (CommonUtil.IsNullOrEmpty(BillingTargetCode))
                {
                    if (String.IsNullOrEmpty(BillingClientCode))
                    {
                        lstControl.Add("BillingClientCode");

                    }
                    if (String.IsNullOrEmpty(BillingTargetNo))
                    {
                        lstControl.Add("BillingTargetNo");
                    }
                    if (lstControl.Count > 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                           ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                           MessageUtil.MODULE_COMMON,
                                           MessageUtil.MessageList.MSG0007,
                                           new string[] { "lblBillingTargetCode" },
                                             lstControl.ToArray());

                        return Json(res);
                    }
                }

                //Retrieve billing target data
                string strBillingTargetCode = BillingTargetCode;
                if (CommonUtil.IsNullOrEmpty(strBillingTargetCode)
                    || strBillingTargetCode.Length != 14)
                {
                    strBillingTargetCode = comUtil.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) +
                                                "-" + BillingTargetNo;
                }

                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                sParam.doBillingTarget = handler.GetTbt_BillingTargetForViewData(strBillingTargetCode, MiscType.C_CUST_TYPE);
                if (sParam.doBillingTarget == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6040, null, null);
                    return Json(res);
                }
                else
                {
                    if(!BLS020_ValidExistOffice(sParam.doBillingTarget.BillingOfficeCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0063,
                                            null,
                                             new string[] { "BillingClientCode", "BillingTargetNo" });
                        return Json(res);
                    }
                    sParam.doBillingTarget.BillingClientCode = comUtil.ConvertBillingClientCode(sParam.doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    res.ResultData = sParam.doBillingTarget;


                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Register billing target
        /// </summary>
        /// <param name="sValid"></param>
        /// <param name="doBillingTarget"></param>
        /// <returns></returns>
        public ActionResult BLS020_RegisterBillingTarge(BLS020_ScreenInputValidate sValid, dtTbt_BillingTargetForView doBillingTarget)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            BLS020_ScreenParameter sParam = GetScreenObject<BLS020_ScreenParameter>();
            tbt_BillingTarget doTbt_BillingTarget = new tbt_BillingTarget();
            IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            try
            {
                ///////// Check Suspending //////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (ModelState.IsValid == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                ///// Check permission //////////
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_TARGET, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                ///////////////5.3 Validate Business
                if (CommonUtil.IsNullOrEmpty(doBillingTarget.IssueInvMonth))
                {
                    doBillingTarget.IssueInvMonth = 0;
                }
                if (doBillingTarget.IssueInvMonth > 60)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                      ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                      MessageUtil.MODULE_BILLING,
                                      MessageUtil.MessageList.MSG6001,
                                      new string[] { "lblIssueInvoiceTiming" },
                                        new string[] { "IssueInvMonth" });
                    return Json(res);
                }

                //check duplicate 
                List<tbt_BillingTarget> lst = new List<tbt_BillingTarget>();
                lst = handler.GetTbt_BillingTarget(null, comUtil.ConvertBillingClientCode(doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG), doBillingTarget.BillingOfficeCode);
                if (lst.Count > 0)
                {
                    doTbt_BillingTarget = lst[0];
                }
                else
                {
                    doTbt_BillingTarget = null;
                }
                //doTbt_BillingTarget = handler.GetTbt_BillingTarget(null, comUtil.ConvertBillingClientCode(doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG), doBillingTarget.BillingOfficeCode);                
                doBillingTarget.BillingTargetCode = comUtil.ConvertBillingClientCode(doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) + "-" + doBillingTarget.BillingTargetNo;
                if (doTbt_BillingTarget != null
                    && doTbt_BillingTarget.BillingTargetCode != doBillingTarget.BillingTargetCode)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    // res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6002, new string[] { doTbtBillingTarget.BillingTargetCode }, new string[] { "BillingClientCodeView" });
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                MessageUtil.MODULE_BILLING,
                                MessageUtil.MessageList.MSG6002,
                                  new string[] { comUtil.ConvertBillingTargetCode(doTbt_BillingTarget.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT) },
                                  new string[] { "BillingClientCode", "BillingTargetNo", "BillingOfficeCode" });

                    return Json(res);
                }
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Reset session of screen parameter
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS020_ResetSession()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                BLS020_ScreenParameter sParam = GetScreenObject<BLS020_ScreenParameter>();
                //BLS020_ScreenInputValidate sValid = GetScreenObject<BLS020_ScreenInputValidate>();
                //sValid.BillingClientCode = null;
                //sValid.BillingOfficeCode = null;
                //sValid.BillingTargetNo = null;
                //sParam.doBillingTarget = null;  
                //ResetSessionData();                             
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Confirm for edit billing target
        /// </summary>
        /// <param name="doBillingTarget"></param>
        /// <returns></returns>
        public ActionResult BLS020_ConfirmEditBillingTarget(dtTbt_BillingTargetForView doBillingTarget)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            BLS020_ScreenParameter sParam = GetScreenObject<BLS020_ScreenParameter>();
            tbt_BillingTarget doTbt_BillingTarget = new tbt_BillingTarget();
            IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            try
            {
                ///////// Check Suspending //////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (ModelState.IsValid == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                ///// Check permission //////////
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_TARGET, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                ///////////////5.3 Validate Business               
                //check duplicate 
                List<tbt_BillingTarget> lst = new List<tbt_BillingTarget>();
                lst = handler.GetTbt_BillingTarget(null, comUtil.ConvertBillingClientCode(doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG), doBillingTarget.BillingOfficeCode);
                if (lst.Count > 0)
                {
                    doTbt_BillingTarget = lst[0];
                }
                else
                {
                    doTbt_BillingTarget = null;
                }
                //doTbt_BillingTarget = handler.GetTbt_BillingTarget(null, comUtil.ConvertBillingClientCode(doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG), doBillingTarget.BillingOfficeCode);
                doBillingTarget.BillingTargetCode = comUtil.ConvertBillingClientCode(doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) + "-" + doBillingTarget.BillingTargetNo;
                if (doTbt_BillingTarget != null
                    && doTbt_BillingTarget.BillingTargetCode != doBillingTarget.BillingTargetCode)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                MessageUtil.MODULE_BILLING,
                                MessageUtil.MessageList.MSG6002,
                                  new string[] { doTbt_BillingTarget.BillingTargetCode },
                                  new string[] { "BillingClientCode", "BillingTargetNo", "BillingOfficeCode" });

                    return Json(res);
                }
                //Update Billing target
                doTbt_BillingTarget = new tbt_BillingTarget();
                doTbt_BillingTarget.BillingTargetCode = doBillingTarget.BillingTargetCode;
                doTbt_BillingTarget.BillingTargetNo = doBillingTarget.BillingTargetNo;
                doTbt_BillingTarget.BillingClientCode = comUtil.ConvertBillingClientCode(doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                doTbt_BillingTarget.BillingOfficeCode = doBillingTarget.BillingOfficeCode;
                doTbt_BillingTarget.ContactPersonName = doBillingTarget.ContactPersonName;
                doTbt_BillingTarget.Memo = doBillingTarget.Memo;
                doTbt_BillingTarget.IssueInvTime = doBillingTarget.IssueInvTime;
                doTbt_BillingTarget.IssueInvMonth = doBillingTarget.IssueInvMonth;
                doTbt_BillingTarget.IssueInvDate = doBillingTarget.IssueInvDate;
                doTbt_BillingTarget.InvFormatType = doBillingTarget.InvFormatType;
                doTbt_BillingTarget.SignatureType = doBillingTarget.SignatureType;
                doTbt_BillingTarget.DocLanguage = doBillingTarget.DocLanguage;
                doTbt_BillingTarget.ShowDueDate = doBillingTarget.ShowDueDate;
                doTbt_BillingTarget.IssueReceiptTiming = doBillingTarget.IssueReceiptTiming;
                doTbt_BillingTarget.ShowAccType = doBillingTarget.ShowAccType;
                doTbt_BillingTarget.WhtDeductionType = doBillingTarget.WhtDeductionType;
                doTbt_BillingTarget.ShowIssueDate = doBillingTarget.ShowIssueDate;
                doTbt_BillingTarget.PayByChequeFlag = doBillingTarget.PayByChequeFlag;
                doTbt_BillingTarget.ShowInvWHTFlag = doBillingTarget.ShowInvWHTFlag;
                doTbt_BillingTarget.SeparateInvType = doBillingTarget.SeparateInvType;
                doTbt_BillingTarget.SuppleInvAddress = doBillingTarget.SuppleInvAddress;

                // tt
                doTbt_BillingTarget.RealBillingClientNameEN = doBillingTarget.RealBillingClientNameEN;
                doTbt_BillingTarget.RealBillingClientNameLC = doBillingTarget.RealBillingClientNameLC;
                doTbt_BillingTarget.RealBillingClientAddressEN = doBillingTarget.RealBillingClientAddressEN;
                doTbt_BillingTarget.RealBillingClientAddressLC = doBillingTarget.RealBillingClientAddressLC;

                // Akat K. 2014-05-23 : update PrintAdvanceDate
                doTbt_BillingTarget.PrintAdvanceDate = doBillingTarget.PrintAdvanceDate;

                using (TransactionScope scope = new TransactionScope())
                {
                handler.UpdateTbt_BillingTarget(doTbt_BillingTarget);

                    // Akat K. 2014-05-21 also update billing office
                    handler.UpdateDebtTracingOffice(doBillingTarget.BillingTargetCode, doBillingTarget.BillingOfficeCode);

                  scope.Complete();
                }
                sParam.doBillingTarget = doBillingTarget;
                sParam.doTbt_BillingTarget = doTbt_BillingTarget;
                res.ResultData = sParam.doBillingTarget;
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, null, null);
                return Json(res);
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
