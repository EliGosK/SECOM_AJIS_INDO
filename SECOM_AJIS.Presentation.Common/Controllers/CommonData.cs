using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;

using CSI.WindsorHelper;

using System.Configuration;
using SECOM_AJIS.Common.Controllers;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SECOM_AJIS.Common.Util.ConstantValue;

using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {

        public ActionResult CMR010_IssueList()
        {
            try
            {
                ICommonHandler CommonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                
                List<dtIssueListData> rptList = CommonHandler.GetTmpIssueListData();
               

                ReportDocument rptH = new ReportDocument();
                string path = ReportUtil.GetReportPath("Reports/CMR010_IssueList.rpt", Server.MapPath("/"));
                //string path = ReportUtil.GetReportTemplatePath("CTR060_CancelContractMemorandum.rpt");

                rptH.Load(path);


                rptH.SetDataSource(rptList);
                //rptH.Subreports["CTR060_1"].SetDataSource(rptListDetail);

                //rptH.SetParameterValue("AutoBillingTypeNone", AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_NONE, "CTR060_1");
                //rptH.SetParameterValue("AutoBillingTypeAll", AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_ALL, "CTR060_1");
                //rptH.SetParameterValue("AutoBillingTypePartial", AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_PARTIAL, "CTR060_1");
                //rptH.SetParameterValue("BankBillingTypeNone", BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_NONE, "CTR060_1");
                //rptH.SetParameterValue("BankBillingTypeAll", BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_ALL, "CTR060_1");
                //rptH.SetParameterValue("BankBillingTypePartial", BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_PARTIAL, "CTR060_1");
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                rptH.Close();               
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }






    }

}
