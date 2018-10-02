using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Inventory.Models;
using System.Transactions;
using System.IO;


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {

        // Akat K. Test Download Report
        public void IVR_TestDownload(string strSessionKey)
        {
            IInventoryDocumentHandler invH = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "test_download_slip.pdf");
            Response.ContentType = "application/pdf";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");

            Stream streamFile = invH.GenerateIVR010("50400520120201" ,"" ,CommonUtil.dsTransData.dtUserData.EmpNo ,CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            using (MemoryStream ms = new MemoryStream()) {
                streamFile.CopyTo(ms);
                Response.BinaryWrite(ms.ToArray());
            }
            Response.End();
        }

    }

}


