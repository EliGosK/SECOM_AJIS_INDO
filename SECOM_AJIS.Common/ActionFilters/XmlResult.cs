using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace SECOM_AJIS.Common.ActionFilters
{
    public class XmlResult : ActionResult
    {
        private readonly object _data;
        public XmlResult(object data)
        {
            _data = data;
        }
        
        public override void ExecuteResult(ControllerContext context)
        {
            if (this._data != null)
            {
                context.HttpContext.Response.Clear();
                XmlRootAttribute root = new XmlRootAttribute("response");

                var xs = new System.Xml.Serialization.XmlSerializer(this._data.GetType(), root);
                context.HttpContext.Response.ContentType = "text/xml";

                xs.Serialize(context.HttpContext.Response.Output, this._data);
            }
        }
    }
}