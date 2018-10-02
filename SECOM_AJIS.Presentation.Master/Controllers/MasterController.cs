using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using System.Globalization;


namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        #region Customer Autocomplete

        /// <summary>
        /// Get name EN of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerNameEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtCustNameEN> lst = hand.GetCustNameEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.CustNameEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get name LC of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerNameLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtCustNameLC> lst = hand.GetCustNameLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.CustNameLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get address EN of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerAddressEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustAddressEN> lst = hand.GetCustAddressEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AddressEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get address LC of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerAddressLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustAddressLC> lst = hand.GetCustAddressLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AddressLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get alley EN of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerAlleyEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustAlleyEN> lst = hand.GetCustAlleyEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AlleyEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get alley LC of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerAlleyLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustAlleyLC> lst = hand.GetCustAlleyLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AlleyLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get road EN of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerRoadEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustRoadEN> lst = hand.GetCustRoadEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.RoadEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get road LC of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerRoadLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustRoadLC> lst = hand.GetCustRoadLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.RoadLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get sub district EN of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerSubDistrictEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustSubDistrictEN> lst = hand.GetCustSubDistrictEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.SubDistrictEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get sub district LC of Customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerSubDistrictLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doCustSubDistrictLC> lst = hand.GetCustSubDistrictLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.SubDistrictLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Site Autocomplete

        /// <summary>
        /// Get name EN of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteNameEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteNameEN> lst = hand.GetSiteNameEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.SiteNameEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get name LC of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteNameLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteNameLC> lst = hand.GetSiteNameLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.SiteNameLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get address EN of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteAddressEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteAddressEN> lst = hand.GetSiteAddressEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AddressEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get address LC of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteAddressLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteAddressLC> lst = hand.GetSiteAddressLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AddressLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get alley EN of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteAlleyEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteAlleyEN> lst = hand.GetSiteAlleyEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AlleyEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get alley LC of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteAlleyLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteAlleyLC> lst = hand.GetSiteAlleyLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.AlleyLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get road EN of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteRoadEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteRoadEN> lst = hand.GetSiteRoadEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.RoadEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get road LC of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteRoadLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteRoadLC> lst = hand.GetSiteRoadLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.RoadLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get sub district EN of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteSubDistrictEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteSubDistrictEN> lst = hand.GetSiteSubDistrictEN(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.SubDistrictEN);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get sub district LC of Site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSiteSubDistrictLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtSiteSubDistrictLC> lst = hand.GetSiteSubDistrictLC(cond);

                List<string> strList = new List<string>();
                foreach (var l in lst)
                {
                    strList.Add(l.SubDistrictLC);
                }
                res.ResultData = strList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        #endregion

        /// <summary>
        /// Get Amphor EN data
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAmphorEN(string provinceCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_District> lst = hand.GetTbm_District(provinceCode);

                var sortedList = from p in lst
                                 orderby p.DistrictNameEN
                                 select p;

                lst = sortedList.ToList<tbm_District>();

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_District>(lst, "DistrictNameEN", "DistrictCode");
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get Amphor LC data
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAmphorLC(string provinceCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_District> lst = hand.GetTbm_District(provinceCode);

                //var sortedList = from p in lst
                //                 orderby p.DistrictNameLC
                //                 select p;

                //lst = sortedList.ToList<tbm_District>();
                CultureInfo culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                lst = lst.OrderBy(p => p.DistrictNameLC, StringComparer.Create(culture, false)).ToList();

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_District>(lst, "DistrictNameLC", "DistrictCode");
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get current language of Amphor data
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAmphorCurrentLanguage(string provinceCode)
        {
            if (CommonUtil.CurrentLanguage(false) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1)
                return this.GetAmphorEN(provinceCode);
            else
                return this.GetAmphorLC(provinceCode);
        }

        /// <summary>
        /// Get all Amphor EN data
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAmphorENFirstElementAll(string provinceCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_District> lst = hand.GetTbm_District(provinceCode);

                var sortedList = from p in lst
                                 orderby p.DistrictNameEN
                                 select p;

                lst = sortedList.ToList<tbm_District>();

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_District>(lst, "DistrictNameEN", "DistrictCode",true,CommonUtil.eFirstElementType.All);
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get all Amphor LC data
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAmphorLCFirstElementAll(string provinceCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_District> lst = hand.GetTbm_District(provinceCode);

                //var sortedList = from p in lst
                //                 orderby p.DistrictNameLC
                //                 select p;

                //lst = sortedList.ToList<tbm_District>();
                CultureInfo culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                lst = lst.OrderBy(p => p.DistrictNameLC, StringComparer.Create(culture, false)).ToList();

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_District>(lst, "DistrictNameLC", "DistrictCode",true,CommonUtil.eFirstElementType.All);
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get all current language of Amphor data
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAmphorCurrentLanguageFirstElementAll(string provinceCode)
        {
            if (CommonUtil.CurrentLanguage(false) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1)
                return this.GetAmphorENFirstElementAll(provinceCode);
            else
                return this.GetAmphorLCFirstElementAll(provinceCode);
        }

        /// <summary>
        /// Get name of Employee data
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public ActionResult GetEmployeeName(string empNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (empNo != null)
                {
                    IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                    List<tbm_Employee> empLst = new List<tbm_Employee>();
                    empLst.Add(new tbm_Employee()
                    {
                        EmpNo = empNo
                    });
                    List<tbm_Employee> lst = handler.GetEmployeeList(empLst);
                    if (lst.Count > 0)
                        res.ResultData = lst[0].EmpFullName;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get active name of Employee data
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public ActionResult GetActiveEmployeeName(string empNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (empNo != null)
                {
                    IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                    List<tbm_Employee> empLst = new List<tbm_Employee>();
                    empLst.Add(new tbm_Employee()
                    {
                        EmpNo = empNo
                    });
                    List<doActiveEmployeeList> lst = handler.GetActiveEmployeeList(empLst);
                    if (lst.Count > 0)
                        res.ResultData = lst[0].EmpFullName;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get name of Supplier data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSupplierName(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<string> lst = hand.GetSupplierName(cond);


                res.ResultData = lst;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get code of Instrument data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetInstrumentCode(string cond) //Add by Jutarat A. on 24032014
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                res.ResultData = hand.GetInstrumentCode(cond);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get code of Instrument data (all available)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetInstrumentCodeAll(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                res.ResultData = hand.GetInstrumentCodeAll(cond, null);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get code of Instrument data (Parent Expansion)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetInstrumentCodeExpansionParent(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                res.ResultData = hand.GetInstrumentCodeAll(cond, ExpansionType.C_EXPANSION_TYPE_PARENT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get code of Instrument data (Child Expansion)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetInstrumentCodeExpansionChild(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                res.ResultData = hand.GetInstrumentCodeAll(cond, ExpansionType.C_EXPANSION_TYPE_CHILD);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}
