using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Billing.Models;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        /// <summary>
        /// Check permission for access screen BLS080
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS080_Authority(BLS080_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            // Check permission
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_DOWNLOD_AUTO, FunctionID.C_FUNC_ID_DOWNLOAD) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<object>("BLS080", param, res);
        }

        /// <summary>
        /// Method for return view of screen BLS080
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS080")]
        public ActionResult BLS080()
        {
            return View();
        }

        /// <summary>
        /// Initial grid of screen BLS030 
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS080_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS080_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get auto transfer bank file list (csv)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult BLS080_SearchResponse(BLS080_SearchCondition cond)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<dtDownloadAutoTransferBankFile> list = new List<dtDownloadAutoTransferBankFile>();

            try
            {
                // Check required field.
                //if (ModelState.IsValid == false)
                //{
                //    ValidatorUtil.BuildErrorMessage(res, this);
                //    if (res.IsError)
                //        return Json(res);

                //}

                if (CommonUtil.IsNullAllField(cond))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                }

                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                list = handlerBilling.GetDownloadAutoTransferBankFile(cond.SecomAccountID, cond.AutoTranferDateFrom, cond.AutoTranferDateTo, cond.GeneateDateFrom, cond.GeneateDateTo);

            }
            catch (Exception ex)
            {


                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtDownloadAutoTransferBankFile>(list, "Billing\\BLS080_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        /// <summary>
        /// Check exist file (BLS080)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult BLS080_CheckExistFile(string fileName)
        {
            try
            {

                string path = PathUtil.GetPathValue(PathUtil.PathName.AutoTransferFile, fileName);// ReportUtil.GetGeneratedReportPath(fileName);
                if (System.IO.File.Exists(path) == true)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Download file and wirte history to download log
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult BLS080_DownloadAndWriteLog(string fileName)
        {
            try
            {

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = fileName,
                    DocumentCode = ReportID.C_REPORT_ID_AUTO_TRANSFER_BANK_FILE,
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
                };


                ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                int isSuccess = handlerLog.WriteDocumentDownloadLog(doDownloadLog);

                string fiilPath = PathUtil.GetPathValue(PathUtil.PathName.AutoTransferFile, fileName);// ReportUtil.GetGeneratedReportPath(fileName);

                return File(fiilPath, "text/csv", fileName);


            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }




    }



}
