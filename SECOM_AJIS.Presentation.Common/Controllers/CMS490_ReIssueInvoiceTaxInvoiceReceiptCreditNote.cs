using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Billing;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;
using System.Transactions;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS490
        /// </summary>
        /// <param name="param">Screen parameter</param>
        /// <returns></returns>
        public ActionResult CMS490_Authority(CMS490_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // - Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_CARRY_OVER_AND_PROFIT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }                                             
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS490_ScreenParameter>("CMS490", param, res);
        }

        /// <summary>
        /// Initialize screen CMS490
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS490")]
        public ActionResult CMS490()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil cm = new CommonUtil();
            try
            {
                CMS490_ScreenParameter param = GetScreenObject<CMS490_ScreenParameter>();

                // Prepare for show section                   
                ViewBag.txtCallerScreenId = param.CallerScreenID;

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        public ActionResult CMS490_UpdateInvoice(string DocNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            CMS490_ValidateData(validator,DocNo,"Invoice");
            ValidatorUtil.BuildErrorMessage(res, validator, null);
            if (res.IsError)
            {
                return Json(res);
            }

            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            string UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            string message = handler.GetReIssueInvoice(DocNo, UpdateBy);
            if (message == "Invalid Document no." || DocNo =="")
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0162, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "This Document no. has been canceled")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0163, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "Please check BillingTargetCode")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0164);
                res.ResultData = false;
            }
            else if (message == "Print")
            {
                res.ResultData = true;
            }
            return Json(res);
        }

        public ActionResult CMS490_UpdateTaxInvoice(string DocNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            CMS490_ValidateData(validator, DocNo, "TaxInvoice");
            ValidatorUtil.BuildErrorMessage(res, validator, null);
            if (res.IsError)
            {
                return Json(res);
            }

            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            string UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            string message = handler.GetReIssueTaxInvoice(DocNo, UpdateBy);
            if (message == "Invalid Document no." || DocNo == "")
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0162, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "This Document no. has been canceled")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0163, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "Please check BillingTargetCode")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0164);
                res.ResultData = false;
            }
            else if (message == "Print")
            {
                res.ResultData = true;
            }
            return Json(res);
        }

        public ActionResult CMS490_UpdateReceipt(string DocNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            CMS490_ValidateData(validator, DocNo, "Receipt");
            ValidatorUtil.BuildErrorMessage(res, validator, null);
            if (res.IsError)
            {
                return Json(res);
            }

            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            string UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            string message = handler.GetReIssueReceipt(DocNo, UpdateBy);
            if (message == "Invalid Document no." || DocNo == "")
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0162, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "This Document no. has been canceled")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0163, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "Please check BillingTargetCode")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0164);
                res.ResultData = false;
            }
            else if (message == "Print")
            {
                res.ResultData = true;
            }
            return Json(res);
        }

        public ActionResult CMS490_UpdateCreditNote(string DocNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            CMS490_ValidateData(validator, DocNo, "CreditNote");
            ValidatorUtil.BuildErrorMessage(res, validator, null);
            if (res.IsError)
            {
                return Json(res);
            }

            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            string UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            string message = handler.GetReIssueCreditNote(DocNo, UpdateBy);
            if (message == "Invalid Document no." || DocNo == "")
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0162, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "This Document no. has been canceled")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0163, new string[] { DocNo }, new string[] { DocNo });
                res.ResultData = false;
            }
            else if (message == "Please check BillingTargetCode")
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0164);
                res.ResultData = false;
            }
            else if (message == "Print")
            {
                res.ResultData = true;
            }
            return Json(res);
        }

        public ActionResult CMS490_blank(string DocNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            CMS490_ValidateData(validator, DocNo, "");
            ValidatorUtil.BuildErrorMessage(res, validator, null);
            if (res.IsError)
            {
                return Json(res);
            }

            res.ResultData = false;
            return Json(res);
        }

        private void CMS490_ValidateData(ValidatorUtil validator, string DocNo, string DocType)
        {
            if (CommonUtil.IsNullOrEmpty(DocType))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_COMMON, "CMS490"
                  , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                  , "txtDocType", "lblDocType", "txtDocType");                 // id, message, id that color is changed
            }
            if (CommonUtil.IsNullOrEmpty(DocNo))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_COMMON, "CMS490"
                  , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                  , "txtDocNo", "lblDocNo", "txtDocNo");                 // id, message, id that color is changed
            }
            
        }
    }
}
