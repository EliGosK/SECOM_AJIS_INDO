using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Collections.Generic;
using System.IO;
using SECOM_AJIS.Common.ActionFilters;
using System.Xml;
using NLog;

namespace SECOM_AJIS.Common.Controllers
{

    public class TestDTO
    {
        public int id { get; set; }
        public string description { get; set; }
        public DateTime startDate { get; set; }
        public IList<TestDetail> detail { get; set; }
    }
    public class TestDetail
    {
        public int id { get; set; }
        public string value { get; set; }
    }
    
    public class TestController : BaseController
    {
        public void TestPassJSONDownload(TestDTO data)
        {
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "test.txt");
            Response.ContentType = "text/xml"; //"application/force-download";
            Response.Charset = "windows-874";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding(874); //System.Text.Encoding.GetEncoding("UTF-8");
            //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(data.GetType());
            //StringWriter sw = new StringWriter();
            //x.Serialize(sw, data);
            Response.Write(string.Format("{0},{1},{2}", data.id, data.description, data.startDate.ToString()));
            Response.End();
            
        }
        public string TestPassJSON(TestDTO data)
        {
            string s="No Data Passed";
            if (data != null)
            {
                s = data.description;
                int detailCount = 0;
                if (data.detail != null)
                {
                     detailCount=data.detail.Count;
                }
                s += " with detail count:" + detailCount;

            }
                
            
            return s;
        }
        public string TestNLog()
        {
            int i = 0, j = 5;
            try
            {
                i = j / i;
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.ErrorException(ex.Message, ex);
            }
            return "TestNLog";
        }
        public void WriteTableErrorLog(DateTime dt, Exception ex)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            Exception logEx = ex;

            while (logEx.InnerException != null)
            {
                logEx = logEx.InnerException;
            }
            var logEventInfo = new LogEventInfo(LogLevel.Error, "databaselog", "logMessage");
            logEventInfo.Properties["ErrorDescription"] = logEx.Message;
            logEventInfo.Properties["CreateDate"] = dt;
            logEventInfo.Exception = ex;
            logger.Log(logEventInfo);

        }
        public ActionResult TestPopupDialog()
        {
            int i = 0, j = 5;
            try
            {
                i = j / i;
            }
            catch (Exception ex)
            {
                WriteTableErrorLog(DateTime.Now, ex);
            }
            return View();
        }
        public ActionResult TestGrid()
        {
            return View();
        }


        public ActionResult TestControl_Authority()
        {
            return InitialScreenEnvironment<object>("TestControl", null);
        }
        [Initialize("TestControl")]
        public ActionResult TestControl()
        {
            return View();
        }
        public string CsvHeaderGrid(string header)
        {
            string csvHeader = CommonUtil.CsvHeaderGrid(header);
           
            return csvHeader;
        }
        public ActionResult InitialGrid()
        {

            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS090"));
        }
        public ActionResult Language()
        {
            return View();
        }
        public string ChangeSession(string key, string value, string k)
        {
            if (!string.IsNullOrEmpty(key))
                Session[k + key] = value;
            return "success";
        }
        public string GetSession(string key)
        {
            return Session[key].ToString();
        }
        public ActionResult HtmlHelper_Authority()
        {
            ObjectResultData res = new ObjectResultData();
            return InitialScreenEnvironment<object>("HtmlHelper", null);
        }
        [Initialize("HtmlHelper")]
        public ActionResult HtmlHelper()
        {
            return View();
        }
        public ActionResult Dialog()
        {
            return View();
        }

        public ActionResult AjaxCall_Authority()
        {
            return InitialScreenEnvironment<object>("AjaxCall",null);
        }

        [Initialize("AjaxCall")]
        public ActionResult AjaxCall()
        {
            return View();
        }
        public ActionResult Upload()
        {

            return View();
        }
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                
            }
            return View();
        }
    }
}
