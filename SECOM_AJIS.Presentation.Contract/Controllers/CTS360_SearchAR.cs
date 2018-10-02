//*********************************
// Create by: Akat K.
// Create date: 13/Sep/2011
// Update date: 13/Sep/2011
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController {

        private const string CTS360_Screen = "CTS360";
        private const string C_APPROVER = "1";
        private const string C_AUDITOR = "2";
        private const string C_REQUESTER = "3";

        /// <summary>
        /// Check user’s permission and view AR office data from summary screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS360_Authority(CTS360_ScreenParameter param) {
            ObjectResultData res = new ObjectResultData();
            try {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_AR)) {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_AR, FunctionID.C_FUNC_ID_VIEW_AR_OFFICE)
                    && ScreenID.C_SCREEN_ID_SUMMARY_AR.Equals(param.screenMode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (res.IsError) {
                    return Json(res);
                }

                return InitialScreenEnvironment<CTS360_ScreenParameter>(CTS360_Screen, param, res);
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize(CTS360_Screen)]
        public ActionResult CTS360() {
            CTS360_ScreenParameter param = GetScreenObject<CTS360_ScreenParameter>();

            ViewBag.AROfficeCode = param.AROfficeCode;

            if (C_REQUESTER.Equals(param.SubObjectID)) {
                ViewBag.ScreenMode = "Requester";
                ViewBag.DefaultRole = ARRole.C_AR_ROLE_REQUESTER;
                ViewBag.DefaultStatus = ARSearchStatus.C_AR_SEARCH_STATUS_HANDLING;
                ViewBag.DefaultPeriod = ARSearchPeriod.C_AR_SEARCH_PERIOD_REQUEST_DATE;
            } else if (C_APPROVER.Equals(param.SubObjectID)) {
                ViewBag.ScreenMode = "Approver";
                ViewBag.DefaultRole = ARRole.C_AR_ROLE_APPROVER;
                ViewBag.DefaultStatus = ARSearchStatus.C_AR_SEARCH_STATUS_HANDLING;
                ViewBag.DefaultPeriod = ARSearchPeriod.C_AR_SEARCH_PERIOD_REQUEST_DATE;
            } else if (C_AUDITOR.Equals(param.SubObjectID)) {
                ViewBag.ScreenMode = "Auditor";
                ViewBag.DefaultRole = ARRole.C_AR_ROLE_AUDITOR;
                ViewBag.DefaultStatus = ARSearchStatus.C_AR_SEARCH_STATUS_HANDLING;
                ViewBag.DefaultPeriod = ARSearchPeriod.C_AR_SEARCH_PERIOD_REQUEST_DATE;
            } else if (ScreenID.C_SCREEN_ID_SUMMARY_AR.Equals(param.screenMode) && ViewBag.AROfficeCode != null) {
                ViewBag.ScreenMode = "Office";
            } else {
                ViewBag.ScreenMode = "Search";
            }

            //ViewBag.DueDate1Week = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1WEEK;
            //ViewBag.DueDate2Week = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS;
            //ViewBag.DueDate1Month = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1MONTH;

            ViewBag.DefaultFrom = "";//DateTime.Now.ToString("dd-MMM-yyyy"); Modify by Thanawit S.
            ViewBag.DefaultTo = "";//DateTime.Now.ToString("dd-MMM-yyyy"); //DateTime.Now.AddDays(14.0).ToString("dd-MMM-yyyy"); Modify by Thanawit S.

            ViewBag.UnImplemented = ContractStatus.C_CONTRACT_STATUS_BEF_START;
            ViewBag.Implemented = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
            ViewBag.StopService = ContractStatus.C_CONTRACT_STATUS_STOPPING;
            ViewBag.Cancel = ContractStatus.C_CONTRACT_STATUS_END + ","
                            + "," + ContractStatus.C_CONTRACT_STATUS_CANCEL + ","
                            + "," + ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL;

            ViewBag.AL = ProductType.C_PROD_TYPE_AL + "," + ProductType.C_PROD_TYPE_RENTAL_SALE + "," + ProductType.C_PROD_TYPE_ONLINE;
            ViewBag.Sales = ServiceType.C_SERVICE_TYPE_SALE;
            ViewBag.Maintenance = ProductType.C_PROD_TYPE_MA;
            ViewBag.SentryGuard = ProductType.C_PROD_TYPE_BE + "," + ProductType.C_PROD_TYPE_SG;

            ViewBag.Handling = ","+ARStatus.C_AR_STATUS_AUDITING+","+ARStatus.C_AR_STATUS_RETURNED_REQUEST+","+ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL+",";
            ViewBag.Complete = ","+ARStatus.C_AR_STATUS_INSTRUCTED+","+ARStatus.C_AR_STATUS_REJECTED+","+ARStatus.C_AR_STATUS_APPROVED+",";

            return View();
        }

        /// <summary>
        ///  Initial grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS360_InitGrid() {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS360"));
        }

        /// <summary>
        /// Search AR by role when change mode to "Requester List Mode", "Approver List Mode", or "Auditor List Mode"
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult CTS360_ARListByRole(doSearchARListByRole condition) {
            ObjectResultData res = new ObjectResultData();
            
            try {
                if (condition.ARSpecifyPeriod == null && (condition.ARSpecifyPeriodFrom.HasValue || condition.ARSpecifyPeriodTo.HasValue)) {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0116, null, new string[] { "cboARSpecifyPeriod" });
                }

                if (condition.ARSpecifyPeriod != null &&
                    condition.ARSpecifyPeriodFrom != null && condition.ARSpecifyPeriodTo != null &&
                    condition.ARSpecifyPeriodFrom.Value.CompareTo(condition.ARSpecifyPeriodTo.Value) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0090, null, new string[] { "dateByRoldPeriodFrom", "dateByRolePeriodTo" });
                }

                if (res.IsError) {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IARHandler hand = ServiceContainer.GetService<IARHandler>() as IARHandler;
                List<dtARList> list = hand.GetARListByRole(condition);
                CommonUtil.MappingObjectLanguage<dtARList>(list);

                MiscTypeMappingList miscMapList = new MiscTypeMappingList();
                miscMapList.AddMiscType(list.ToArray());
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comh.MiscTypeMappingList(miscMapList);

                foreach (var ar in list) {
                    setColStyle(ar);
                }

                string xml = CommonUtil.ConvertToXml<dtARList>(list, "Contract\\CTS360", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search AR by criteria when click [Search] button on AR list by role section
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult CTS360_SearchAR(SearchARCondition condition) {
            ObjectResultData res = new ObjectResultData();

            try {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { condition });

                if (condition.SpecfyPeriod == null && (condition.SpecifyPeriodFrom.HasValue || condition.SpecifyPeriodTo.HasValue)) {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0116, null, new string[] { "cboSearchSpecifyPeriod" });
                }

                if (condition.SpecfyPeriod != null &&
                    condition.SpecifyPeriodFrom.HasValue && condition.SpecifyPeriodTo.HasValue &&
                    condition.SpecifyPeriodFrom.Value.CompareTo(condition.SpecifyPeriodTo.Value) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0090, null, new string[] { "dateSearchPeriodFrom", "dateSearchPeriodTo" });
                }

                if (res.IsError) {
                    return Json(res);
                }

                CommonUtil c = new CommonUtil();
                condition.ContractCode = c.ConvertContractCode(condition.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                condition.QuotationTargetCode = c.ConvertQuotationTargetCode(condition.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IARHandler hand = ServiceContainer.GetService<IARHandler>() as IARHandler;
                List<dtARList> list = hand.SearchARList(condition);
                CommonUtil.MappingObjectLanguage<dtARList>(list);

                MiscTypeMappingList miscMapList = new MiscTypeMappingList();
                miscMapList.AddMiscType(list.ToArray());
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comh.MiscTypeMappingList(miscMapList);

                foreach (var ar in list) {
                    setColStyle(ar);
                }

                string xml = CommonUtil.ConvertToXml<dtARList>(list, "Contract\\CTS360", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Setting column and data style (color, and text)
        /// </summary>
        /// <param name="ar"></param>
        private void setColStyle(dtARList ar) {
            if (ar.hasRespondingDetailFlag.HasValue
                //&& ar.hasRespondingDetailFlag.Value
                && ar.hasRespondingDetailFlag.Value != FlagType.C_FLAG_ON)
            {
                ar.OverNewImportant = "n";
            }

            if (ar.DueDateDeadLine.HasValue)
            {
                ar.DueDateDeadLine = new DateTime(ar.DueDateDeadLine.Value.Year
                    , ar.DueDateDeadLine.Value.Month
                    , ar.DueDateDeadLine.Value.Day
                    , 23, 59, 59);

                if (ar.DueDateTime.HasValue)
                {
                    ar.DueDateDeadLine = new DateTime(ar.DueDateDeadLine.Value.Year
                    , ar.DueDateDeadLine.Value.Month
                    , ar.DueDateDeadLine.Value.Day
                    , ar.DueDateTime.Value.Hours
                    , ar.DueDateTime.Value.Minutes
                    , ar.DueDateTime.Value.Seconds);
                }
            }

            //Comment by Jutarat A. on 04092012
            ////if ((ar.DueDateDeadLine.HasValue && ar.DueDateDeadLine.Value.CompareTo(System.DateTime.Today) < 0)
            //if ((ar.DueDateDeadLine.HasValue && ar.DueDateDeadLine.Value.CompareTo(DateTime.Now) < 0)
            //    && (!ar.ARStatus.Equals(ARStatus.C_AR_STATUS_INSTRUCTED)
            //        && !ar.ARStatus.Equals(ARStatus.C_AR_STATUS_REJECTED)
            //        && !ar.ARStatus.Equals(ARStatus.C_AR_STATUS_APPROVED)))
            //{
            //    ar.OverNewImportant = "o";
            //}
            //End Comment

            if (ar.ImportanceFlag.HasValue && ar.ImportanceFlag.Value) {
                ar.Important = "i";
            }

            if (CommonUtil.dsTransData.dtUserData.EmpNo.Equals(ar.ApprEmpNo)
                || CommonUtil.dsTransData.dtUserData.EmpNo.Equals(ar.ReqEmpNo)
                || CommonUtil.dsTransData.dtUserData.EmpNo.Equals(ar.AudEmpNo))
            {
                ar.isStatusYellow = true;
            } else {
                ar.isStatusYellow = false;
            }
        }
    }
}
