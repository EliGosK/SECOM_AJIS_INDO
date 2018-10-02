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
        /// Get District data
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDistrict(string provinceCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_District> lst = handler.GetTbm_District(provinceCode);

                lst = (from p in lst orderby p.DistrictName ascending select p).ToList<tbm_District>();

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_District>(lst, "DistrictName", "DistrictCode");

                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get address of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustAddress(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustAddress> lst = handler.GetCustAddress(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.Address);
                }

                //string xml = CommonUtil.ConvertToXml<doCustAddress>(lst);
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
        /// Get alley of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustAlley(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustAlley> lst = handler.GetCustAlley(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.Alley);
                }

                //string xml = CommonUtil.ConvertToXml<doCustAlley>(lst);
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
        /// Get road of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustRoad(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustRoad> lst = handler.GetCustRoad(cond);


                List<string> strList = new List<string>();
                foreach (doCustRoad r in lst)
                {
                    strList.Add(r.Road);
                }

                //string xml = CommonUtil.ConvertToXml<doCustRoad>(lst);

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
        /// Get sub district of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustSubDistrict(string cond)
        {
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustSubDistrict> lst = handler.GetCustSubDistrict(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.SubDistrict);
                }

                //string xml = CommonUtil.ConvertToXml<doCustSubDistrict>(lst);
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
