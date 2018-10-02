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

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS150_Authority(ICS150_ScreenParameter param)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            ObjectResultData res = new ObjectResultData();

            //if (handlerCommon.IsSystemSuspending())
            //{
            //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
            //    return Json(res);
            //}

            // Check User Permission
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_MATCH_R_REPORT, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<ICS150_ScreenParameter>("ICS150", param, res);

        }

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS150")]
        public ActionResult ICS150()
        {

            ICS150_ScreenParameter param = GetScreenObject<ICS150_ScreenParameter>();

            return View();
        }

        public ActionResult ICS150_ExportExcelData(doMatchRReport dtIncome)
        {
            ICS150_ScreenParameter param = GetScreenObject<ICS150_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            
            try
            {
                ValidatorUtil validator = new ValidatorUtil();
               
                ICS150_ValidateBusiness(validator, dtIncome);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                if (dtIncome == null || CommonUtil.IsNullAllField(dtIncome))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    if (string.IsNullOrEmpty(dtIncome.CreateBy) && CommonUtil.dsTransData != null)
                    {
                        dtIncome.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    }
                    var lst = handler.GetListIRC050(dtIncome);

                    if (lst.Count != 0)
                    {
                        IIncomeDocumentHandler docService = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
                        param.PendingDownloadFilePath = docService.GenerateICR050Report(lst, dtIncome);
                        param.PendingDownloadFileName = "ICR050Report.xlsx";
                        res.ResultData = true;
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    }
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS150_GetGroupName(DateTime? paymentDate, string createBy)
        {
            try
            {
                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                if (string.IsNullOrEmpty(createBy) && CommonUtil.dsTransData != null)
                {
                    createBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<doGetMatchGroupNamePayment> lstGroupname = handler.getMatchGroupNameCbo(paymentDate, createBy);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doGetMatchGroupNamePayment>(lstGroupname, "MatchRGroupName", "MatchRGroupName",true);
                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        public ActionResult ICS150_Download()
        {
            ICS150_ScreenParameter param = GetScreenObject<ICS150_ScreenParameter>();
            if (!string.IsNullOrEmpty(param.PendingDownloadFilePath))
            {
                var stream = new FileStream(param.PendingDownloadFilePath, FileMode.Open, FileAccess.Read);
                param.PendingDownloadFilePath = null;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", param.PendingDownloadFileName);
            }
            else
            {
                ObjectResultData res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(new FileNotFoundException("Report file not found.", param.PendingDownloadFilePath));
                return Json(res);
            }
        }

        private void ICS150_ValidateBusiness(ValidatorUtil validator, doMatchRReport paramsearch)
        {
            ICS150_ScreenParameter screenObject = GetScreenObject<ICS150_ScreenParameter>();



            
            if (paramsearch.PaymentDate == null)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS150"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "PaymentDate", "lblPaymentDate", "PaymentDate");

            }


            if (CommonUtil.IsNullOrEmpty(paramsearch.GroupName))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS150"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "GroupName", "lblGroupname", "GroupName");

            }
           
        }
     
    }
}
