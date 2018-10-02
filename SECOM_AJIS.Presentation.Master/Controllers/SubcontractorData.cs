using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        /// <summary>
        /// Get name of Subcontractor data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSubcontractorName(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doSubcontractorName> lst = handler.GetSubcontractorName(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.SubcontractorName);
                }
                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get name EN of Subcontractor data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSubcontractorNameEN(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doSubcontractorNameEN> lst = handler.GetSubcontractorNameEN(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.SubcontractorName);
                }
                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get name LC of Subcontractor data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSubcontractorNameLC(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doSubcontractorNameLC> lst = handler.GetSubcontractorNameLC(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.SubcontractorName);
                }
                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get address of Subcontractor data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSubcontractorAddress(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doSubcontractorAddress> lst = handler.GetSubcontractorAddress(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.Address);
                }
                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get address EN of Subcontractor data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSubcontractorAddressEN(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doSubcontractorAddressEN> lst = handler.GetSubcontractorAddressEN(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.Address);
                }
                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get address LC of Subcontractor data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSubcontractorAddressLC(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doSubcontractorAddressLC> lst = handler.GetSubcontractorAddressLC(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.Address);
                }
                return Json(strList.ToArray());
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
