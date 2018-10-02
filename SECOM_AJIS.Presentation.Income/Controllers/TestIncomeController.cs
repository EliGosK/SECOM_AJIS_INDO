using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using System.IO;

using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Diagnostics;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Income.Models;


namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        public ActionResult TestIncome_Authority(TestIncome_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            return InitialScreenEnvironment<object>("TestIncome", param , res);
        }

        [Initialize("TestIncome")]
        public ActionResult TestIncome()
        {
            return View();
        }

        public ActionResult ICR010_Receipt(string receiptNo)
        {
            IIncomeDocumentHandler incomeDocHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
            Stream stream2 = incomeDocHandler.GenerateICR010(receiptNo, 
                CommonUtil.dsTransData.dtUserData.EmpNo,
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime); 
            return File(stream2, "application/pdf");
        }

        public ActionResult ICR020_CreditNote(string creditNoteNo)
        {
            IIncomeDocumentHandler incomeDocHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
            Stream stream2 = incomeDocHandler.GenerateICR020(creditNoteNo,
                CommonUtil.dsTransData.dtUserData.EmpNo,
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            return File(stream2, "application/pdf");
        }

        public ActionResult ICR010_GenerateReport(string receiptNo)
        {
            ObjectResultData res = new ObjectResultData();
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            IIncomeDocumentHandler incomeDocHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;

            try
            {
                doReceipt receiptData = incomeHandler.GetReceipt(receiptNo);
                if (receiptData != null)
                {
                    incomeDocHandler.GenerateICR010FilePath(receiptData.ReceiptNo, receiptData.CreateBy, receiptData.CreateDate.Value);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }

    public class TestIncome_ScreenParameter : ScreenParameter
    {

    }


}
