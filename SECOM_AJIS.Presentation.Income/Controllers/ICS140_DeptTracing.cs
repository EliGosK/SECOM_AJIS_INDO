
//*********************************
// Create by: Waroon H.
// Create date: 29/Mar/2012
// Update date: 29/Mar/2012
//*********************************
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
using SECOM_AJIS.Presentation.Income.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
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
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.DataEntity.Inventory;

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS140_Authority(ICS140_ScreenParameter param)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            ObjectResultData res = new ObjectResultData();

            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            // Check User Permission
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_DEBT_TRACING, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            param.IsHQUser = false;
            if (CommonUtil.dsTransData != null && CommonUtil.dsTransData.dtUserData != null)
            {
                var permission = iincomeHandler.GetTbm_DebtTracingPermission(CommonUtil.dsTransData.dtUserData.EmpNo);
                if (permission != null && permission.Count > 0)
                {
                    param.IsHQUser = true;
                }
            }

            return InitialScreenEnvironment<ICS140_ScreenParameter>("ICS140", param, res);

        }

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS140")]
        public ActionResult ICS140()
        {
            var sparam = this.GetScreenObject<ICS140_ScreenParameter>();

            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            sparam.IsHQUser = false;
            if (CommonUtil.dsTransData != null && CommonUtil.dsTransData.dtUserData != null)
            {
                var permission = iincomeHandler.GetTbm_DebtTracingPermission(CommonUtil.dsTransData.dtUserData.EmpNo);
                if (permission != null && permission.Count > 0)
                {
                    sparam.IsHQUser = true;
                }
            }

            ViewBag.IsHQUser = sparam.IsHQUser;

            ViewBag.PaidButtonLabel = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INCOME, ScreenID.C_SCREEN_ID_DEBT_TRACING, "lblPaidButton");
            ViewBag.PaidConfirmParam = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INCOME, ScreenID.C_SCREEN_ID_DEBT_TRACING, "PaidConfirmParam");

            return View();
        }

        public ActionResult ICS140_InitialGridCustList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS140_CustList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult ICS140_InitialGridReturnedCheque()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS140_ReturnedCheque", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult ICS140_InitialInvoiceList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS140_InvoiceList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult ICS140_InitialInvoiceDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS140_InvoiceDetail", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult ICS140_InitialHistory()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS140_History", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult ICS140_RetriveCustList(doDebtTracingCustListSearchCriteria param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var sparam = this.GetScreenObject<ICS140_ScreenParameter>();

                string empno = null;
                if (CommonUtil.dsTransData != null && CommonUtil.dsTransData.dtUserData != null)
                {
                    empno = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                var list = hand.GetDebtTracingCustList(empno, param);

                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS140_CustList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS140_RetriveReturnedCheque(string billingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var sparam = this.GetScreenObject<ICS140_ScreenParameter>();

                var list = hand.GetReturnedCheque(billingTargetCode);
                CommonUtil.MappingObjectLanguage(list);

                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS140_ReturnedCheque", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS140_RetriveInvoiceList(string billingTargetCode, string serviceTypeCode, string debtTracingSubStatus)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var sparam = this.GetScreenObject<ICS140_ScreenParameter>();
                
                string empno = null;
                if (CommonUtil.dsTransData != null && CommonUtil.dsTransData.dtUserData != null)
                {
                    empno = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                var list = hand.GetDebtTracingInvoiceList(billingTargetCode, serviceTypeCode, debtTracingSubStatus, empno);
                CommonUtil.MappingObjectLanguage(list);

                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS140_InvoiceList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS140_RetriveInvoiceDetail(string billingTargetCode, string invoiceNo, int? invoiceOCC)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var sparam = this.GetScreenObject<ICS140_ScreenParameter>();

                var list = hand.GetDebtTracingInvoiceDetail(billingTargetCode, invoiceNo, invoiceOCC);
                CommonUtil.MappingObjectLanguage(list);

                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS140_InvoiceDetail", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS140_RetriveHistory(string billingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var sparam = this.GetScreenObject<ICS140_ScreenParameter>();

                var list = hand.GetDebtTracingHistory(billingTargetCode);
                CommonUtil.MappingObjectLanguage(list);

                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS140_History", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS140_SaveInput(doDebtTracingInput input)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    if (res.IsError)
                        return Json(res);
                }
                
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var sparam = this.GetScreenObject<ICS140_ScreenParameter>();

                // Comment scope by Jirawat Jannet on 2016-10-31
                //using (var scope = new TransactionScope())
                //{
                    handler.SaveDebtTracingInput(input, sparam.IsHQUser);
                //scope.Complete(); // Comment scope by Jirawat Jannet on 2016-10-31

                res.ResultData = true;
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS140_SavePaid(string billingTargetCode, string serviceTypeCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var inventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                var sparam = this.GetScreenObject<ICS140_ScreenParameter>();

                // Coment scopy by Jirawat Jannet  on 2016-10-31
                //using (var scope = new TransactionScope())
                //{
                    doDebtTracingInput input = new doDebtTracingInput()
                    {
                        BillingTargetCode = billingTargetCode, 
                        ServiceTypeCode = serviceTypeCode,
                        Result = DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_MATCH,
                    };

                    int offset;
                    var config = common.GetSystemConfig(ConfigName.C_CONFIG_DEBT_TRACING_WAIT_MATCHING_DAY).FirstOrDefault();
                    if (config == null || !(int.TryParse(config.ConfigValue, out offset)))
                    {
                        offset = 5;
                    }
                    input.NextCallDate = inventory.GetBusinessDateByOffset(DateTime.Today, 5).Value;

                    handler.SaveDebtTracingInput(input, sparam.IsHQUser);

                    doMiscTypeCode misc = new doMiscTypeCode();
                    misc.FieldName = MiscType.C_DEBT_TRACING_STATUS;
                    if (sparam.IsHQUser)
                    {
                        misc.ValueCode = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_PENDING_MATCH;
                    }
                    else
                    {
                        misc.ValueCode = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_PENDING_MATCH;
                    }
                    var status = common.GetMiscTypeCodeList(new List<doMiscTypeCode>() { misc }).FirstOrDefault();

                    res.ResultData = new {
                        DebtTracingSubStatus = misc.ValueCode,
                        DebtTracingStatusDesc = (status != null ? status.ValueDisplay : "")
                    };

                //scope.Complete(); // Coment scopy by Jirawat Jannet  on 2016-10-31
                //} // Coment scopy by Jirawat Jannet  on 2016-10-31
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}
