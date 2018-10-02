//*********************************
// Create by: Attawhit  Chuoosathan
// Create date: 28/Jun/2010
// Update date: 28/Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Master.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Contract;
using System.Text.RegularExpressions;

using System.Transactions;
using System.Reflection;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS010_Screen = "MAS010";

        /// <summary>
        /// - Check user permission for screen MAS010.<br />
        /// - Check system suspending.<br />
        /// - Get building usage from tbm_BuildingUsage.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS010_Authority(MAS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Check system suspending
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_BuildingUsage> ulst = mhandler.GetTbm_BiuldingUsage();
                if (ulst != null && ulst.Count > 0)
                {
                    param.tbm_BuildingUsageList = ulst;
                }
                else
                    param.tbm_BuildingUsageList = new List<tbm_BuildingUsage>();

                // Do in View
                //param.HasEditPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_INFO, FunctionID.C_FUNC_ID_EDIT);
                //param.HasDeletePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_INFO, FunctionID.C_FUNC_ID_DEL);
                //param.UpdateList = new List<doSite>();
                //param.RemoveList = new List<doSite>();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS010_ScreenParameter>(MAS010_Screen, param, res);
        }

        /// <summary>
        /// Initial screen.<br />
        /// Get company type.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS010_Screen)]
        public ActionResult MAS010()
        {
            MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();


            ICustomerMasterHandler custHand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            List<doCompanyType> companyTypeList = custHand.GetCompanyType(null); //Get All Company Type
            MAS010Param.CompanyTypeList = companyTypeList;

            ViewBag.HasEditPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_INFO, FunctionID.C_FUNC_ID_EDIT);
            ViewBag.HasDeletePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_INFO, FunctionID.C_FUNC_ID_DEL);
            MAS010Param.UpdateList = new List<doSite>();
            MAS010Param.RemoveList = new List<doSite>();

            ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_SEARCHPAGE;
            ViewBag.CustTypeJuristic = CustomerType.C_CUST_TYPE_JURISTIC;
            ViewBag.CompTypePublicCoLtd = CompanyType.C_COMPANY_TYPE_PUBLIC_CO_LTD;

            return View(MAS010_Screen);
        }

        /// <summary>
        /// Get config for List of Site table.
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_MAS010()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS010", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for Customer Group table.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS010_InitialCustomerGroupGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS010_CustomerGroup", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get Customer Information and Customer Group Information.
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public ActionResult MAS010_RetrieveCustInfo(string custCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (CommonUtil.IsNullOrEmpty(custCode))
                {
                    string lblCustomerCode = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS010", "lblCustomerCode");
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS010_Screen,
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1039,
                                        new string[] { lblCustomerCode },
                                        new string[] { "Search_CustCode" });
                    return Json(res);
                }
                else
                {
                    ICustomerMasterHandler handlerCustomerMaster = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                    CommonUtil c = new CommonUtil();
                    List<doCustomer> custList = handlerCustomerMaster.GetCustomer(c.ConvertCustCode(custCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    if (custList.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0139);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                    else
                    {
                        MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                        MAS010Param.CustomerData = custList[0];

                        List<dtCustomeGroupData> listCustomerGroup = handlerCustomerMaster.GetCustomeGroupData(custList[0].CustCode, null);
                        MAS010Param.CustomerGruopList_ForView = listCustomerGroup;
                        MAS010Param.CustomerCode = c.ConvertCustCode(custCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                        if (MAS010Param.CustomerCode != null)
                        {
                            MAS010Param.CustomerCode = MAS010Param.CustomerCode.ToUpper();
                        }

                        List<object> result = new List<object>();
                        result.Add(custList[0]);
                        result.Add(listCustomerGroup);

                        // CustomerGruopList
                        IMasterHandler handlerMaster = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                        List<tbm_Group> listGroup = handlerMaster.GetTbm_Group(null);

                        MAS010Param.CustomerGruopList = listGroup;

                        UpdateScreenObject(MAS010Param);
                        res.ResultData = result.ToArray();  // [0] = custList[0] , [1] = listCustomerGroup
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Add/Remove Customer Group from Customer Group List table.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        public ActionResult MAS010_AddRemoveCustomerGroup(string type, string groupCode)
        {
            ObjectResultData res = new ObjectResultData();
            List<object> resultList = new List<object>();
            string result = string.Empty;
            try
            {

                MAS010_ScreenParameter param = GetScreenObject<MAS010_ScreenParameter>();

                if (type == "delete")
                {
                    var deleteItem = (from p in param.CustomerGruopList_ForView where p.GroupCode == groupCode select p).ToList<dtCustomeGroupData>();
                    if (deleteItem.Count > 0)
                    {
                        param.CustomerGruopList_ForView.Remove(deleteItem[0]);

                        result = "1";

                        resultList.Add(result);

                    }
                }
                else if (type == "add")
                {
                    var isExists = (from p in param.CustomerGruopList_ForView where p.GroupCode == groupCode select p).Any();
                    if (isExists)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1007);
                        return Json(res);
                    }

                    // Prepare
                    var groupItem = (from p in param.CustomerGruopList where p.GroupCode == groupCode select p).ToList<tbm_Group>();
                    if (groupItem.Count > 0)
                    {
                        dtCustomeGroupData addItem = new dtCustomeGroupData()
                        {
                            CustCode = param.CustomerCode,
                            GroupCode = groupCode,
                            GroupNameEN = groupItem[0].GroupNameEN,
                            GroupNameLC = groupItem[0].GroupNameLC

                        };

                        param.CustomerGruopList_ForView.Add(addItem);

                        result = "1";

                        resultList.Add(result);
                        resultList.Add(addItem);
                    }

                }

                res.ResultData = resultList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get Site Information of retrieved customer.
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public ActionResult MAS010_RetrieveSiteInfo(String custCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ISiteMasterHandler hand = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                CommonUtil c = new CommonUtil();
                List<doSite> siteList = hand.GetSite(null, c.ConvertCustCode(custCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                MAS010Param.SiteDataList = siteList;

                //Clear site updateList & removeList
                MAS010Param.UpdateList = new List<doSite>();
                MAS010Param.RemoveList = new List<doSite>();

                UpdateScreenObject(MAS010Param);

                res.ResultData = CommonUtil.ConvertToXml<doSite>(siteList, "Master\\MAS010", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Generate company full name for display.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS010_CompanyChange(MAS010_GetCompanyFullName param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                param.FullNameEN = MAS010_DisplayFullName(MAS010Param, param.NameEN, param.CompanyTypeCode, true);
                param.FullNameLC = MAS010_DisplayFullName(MAS010Param, param.NameLC, param.CompanyTypeCode, false);

                res.ResultData = param;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Generate company full name for display only English name.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS010_CustNameENChange(MAS010_GetCompanyFullName param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                param.FullNameEN = MAS010_DisplayFullName(MAS010Param, param.NameEN, param.CompanyTypeCode, true);
                res.ResultData = param;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Generate company full name for display only local name.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS010_CustNameLCChange(MAS010_GetCompanyFullName param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                param.FullNameLC = MAS010_DisplayFullName(MAS010Param, param.NameLC, param.CompanyTypeCode, false);
                res.ResultData = param;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Generate site full address for English address and Local address.
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public ActionResult MAS010_GetSiteFullAddress(MAS010_UpdateSite doUpdate)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                /* --- Validate ---------------------------------------------------------------------- */
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }
                //Set value to ProvinceCode and DistrictCode (EN/LC used for validate only)
                doUpdate.ProvinceCode = doUpdate.ProvinceCodeEN;
                doUpdate.DistrictCode = doUpdate.DistrictCodeEN;

                CommonUtil c = new CommonUtil();
                doUpdate.SiteCode = c.ConvertSiteCode(doUpdate.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                doSite site = new doSite();

                //Update data to do
                site.SiteNameEN = doUpdate.SiteNameEN;
                site.SiteNameLC = doUpdate.SiteNameLC;
                site.SECOMContactPerson = doUpdate.SECOMContactPerson;
                site.PersonInCharge = doUpdate.PersonInCharge;
                site.BuildingUsageCode = doUpdate.BuildingUsageCode;
                site.PhoneNo = doUpdate.PhoneNo;
                site.AddressEN = doUpdate.AddressEN;
                site.AddressLC = doUpdate.AddressLC;
                site.AlleyEN = doUpdate.AlleyEN;
                site.AlleyLC = doUpdate.AlleyLC;
                site.RoadEN = doUpdate.RoadEN;
                site.RoadLC = doUpdate.RoadLC;
                site.SubDistrictEN = doUpdate.SubDistrictEN;
                site.SubDistrictLC = doUpdate.SubDistrictLC;
                site.ProvinceCode = doUpdate.ProvinceCode;
                site.DistrictCode = doUpdate.DistrictCode;
                site.ZipCode = doUpdate.ZipCode;

                IMasterHandler mHand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                doCustomer doCust = CommonUtil.CloneObject<MAS010_UpdateSite, doCustomer>(doUpdate);
                mHand.CreateAddressFull(doCust);

                //set full address
                site.AddressFullEN = doCust.AddressFullEN;
                site.AddressFullLC = doCust.AddressFullLC;

                res.ResultData = site;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Collect changed site information and add to Update List in ScreenObject.
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public ActionResult MAS010_Update(MAS010_UpdateSite doUpdate)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                doUpdate.ProvinceCode = doUpdate.ProvinceCodeEN;
                doUpdate.DistrictCode = doUpdate.DistrictCodeEN;
                /* ----------------------------------------------------------------------------------- */

                CommonUtil c = new CommonUtil();
                doUpdate.SiteCode = c.ConvertSiteCode(doUpdate.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                //Check duplicate site information in same customer
                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();

                //doSite site = MAS010Param.UpdateList.Find(i => i.SiteCode == doUpdate.SiteCode);
                //bool notfound = false;
                //if (site == null)
                //{
                //    notfound = true;
                //    site = MAS010Param.SiteDataList.Find(i => i.SiteCode == doUpdate.SiteCode);
                //}

                // Check duplicate site
                List<doSite> siteList = new List<doSite>();
                if (MAS010Param.SiteDataList != null)
                {
                    foreach (doSite s in MAS010Param.SiteDataList)
                    {
                        siteList.Add(s);
                    }
                }
                if (MAS010Param.UpdateList != null)
                {
                    foreach (doSite s in MAS010Param.UpdateList)
                    {
                        doSite es = null;
                        foreach (doSite os in siteList)
                        {
                            if (os.SiteCode == s.SiteCode)
                            {
                                es = os;
                                break;
                            }
                        }
                        if (es != null)
                            siteList.Remove(es);
                        siteList.Add(s);
                    }
                }
                if (MAS010Param.RemoveList != null)
                {
                    foreach (doSite s in MAS010Param.RemoveList)
                    {
                        doSite es = null;
                        foreach (doSite os in siteList)
                        {
                            if (os.SiteCode == s.SiteCode)
                            {
                                es = os;
                                break;
                            }
                        }
                        if (es != null)
                            siteList.Remove(es);
                    }
                }
                doSite site = null;
                foreach (doSite isite in siteList)
                {
                    if (isite.SiteCode == doUpdate.SiteCode)
                        site = isite;
                    else
                    {
                        int totalDuplicate = 0;

                        string[] checkList = new string[]{
                            "SiteNameLC",
                            "AddressLC",
                            "AlleyLC",
                            "RoadLC",
                            "SubDistrictLC",
                            "ProvinceCode",
                            "DistrictCode"
                        };
                        foreach (string check in checkList)
                        {
                            PropertyInfo propN = doUpdate.GetType().GetProperty(check);
                            PropertyInfo propO = isite.GetType().GetProperty(check);

                            if (propN != null && propO != null)
                            {
                                string txtN = (string)propN.GetValue(doUpdate, null);
                                string txtO = (string)propO.GetValue(isite, null);

                                if (CommonUtil.IsNullOrEmpty(txtN) == false
                                    && CommonUtil.IsNullOrEmpty(txtO) == false)
                                {
                                    txtN = Regex.Replace(txtN, @"[ \p{Z}!\\""#$%&'()*+,-.:;<=>?@^_`{|}\\\[\]]", "");
                                    txtN = txtN.Replace(" ", "");
                                    txtO = Regex.Replace(txtO, @"[ \p{Z}!\\""#$%&'()*+,-.:;<=>?@^_`{|}\\\[\]]", "");
                                    txtO = txtO.Replace(" ", "");

                                    if (txtN == txtO)
                                        totalDuplicate++;
                                }
                                else if (CommonUtil.IsNullOrEmpty(txtN)
                                    && CommonUtil.IsNullOrEmpty(txtO))
                                {
                                    totalDuplicate++;
                                }
                            }
                        }

                        if (totalDuplicate == checkList.Length)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1002);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return Json(res);
                        }
                    }
                }
                if (site == null)
                {
                    site = new doSite();
                    MAS010Param.UpdateList.Add(site);
                }

                //Update data to do
                site.SiteNameEN = doUpdate.SiteNameEN;
                site.SiteNameLC = doUpdate.SiteNameLC;
                site.SECOMContactPerson = doUpdate.SECOMContactPerson;
                site.PersonInCharge = doUpdate.PersonInCharge;
                site.BuildingUsageCode = doUpdate.BuildingUsageCode;
                site.PhoneNo = doUpdate.PhoneNo;
                site.AddressEN = doUpdate.AddressEN;
                site.AddressLC = doUpdate.AddressLC;
                site.AlleyEN = doUpdate.AlleyEN;
                site.AlleyLC = doUpdate.AlleyLC;
                site.RoadEN = doUpdate.RoadEN;
                site.RoadLC = doUpdate.RoadLC;
                site.SubDistrictEN = doUpdate.SubDistrictEN;
                site.SubDistrictLC = doUpdate.SubDistrictLC;
                site.ProvinceCode = doUpdate.ProvinceCode;
                site.DistrictCode = doUpdate.DistrictCode;
                site.ZipCode = doUpdate.ZipCode;

                IMasterHandler mHand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                doCustomer doCust = CommonUtil.CloneObject<MAS010_UpdateSite, doCustomer>(doUpdate);
                mHand.CreateAddressFull(doCust);

                //set full address
                site.AddressFullEN = doCust.AddressFullEN;
                site.AddressFullLC = doCust.AddressFullLC;

                MAS010Param.UpdateList.Add(site);
                UpdateScreenObject(MAS010Param);

                res.ResultData = site;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Add selected site to Remove list in ScreenObject.
        /// </summary>
        /// <param name="siteCodeShort"></param>
        /// <returns></returns>
        public ActionResult MAS010_Remove(string siteCodeShort)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //Check site code is used by other systems
                CommonUtil c = new CommonUtil();
                string siteCodeLong = c.ConvertSiteCode(siteCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                bool blnIsUsed = this.isUsedSite(siteCodeLong);

                if (blnIsUsed)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1001, new string[] { siteCodeShort });
                    return Json(res);
                }
                else
                {
                    MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                    doSite site = MAS010Param.SiteDataList.Find(i => i.SiteCode == siteCodeLong);

                    MAS010Param.RemoveList.Add(site);
                    UpdateScreenObject(MAS010Param);
                    res.ResultData = true;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Validate data of customer before update.<br />
        /// - Check dummy ID.<br />
        /// - Check duplicate ID.<br />
        /// - Check duplicate local name.
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        [PermissionOperationAttibute(Function = FunctionID.FUNC_ID_EDIT)]
        public ActionResult MAS010_ConfirmUpdate(MAS010_UpdateCustomer doUpdate)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                //Check system suspending
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();

                /* --- Validate ---------------------------------------------------------------------- */
                ValidatorUtil validator = new ValidatorUtil(this);

                /* --- Special Validate (if uncheck DummyIDFlag, IDNo must have value) --------------- */
                //if (!doUpdate.DummyIDFlag.Value && CommonUtil.IsNullOrEmpty(doUpdate.IDNo))
                if (doUpdate.DummyIDFlag != null && !doUpdate.DummyIDFlag.Value && CommonUtil.IsNullOrEmpty(doUpdate.IDNo)) //Modify by Jutarat A. on 10022014
                    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                              MAS010_Screen,
                                              MessageUtil.MODULE_COMMON,
                                              MessageUtil.MessageList.MSG0007,
                                              "cust_IDNoID",//solve case duplicate key when AddErrorMessage
                                              "lblIDno_TaxIDno",
                                              "cust_IDNo");

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING; //Add by Jutarat A. on 25122013
                    return Json(res);
                }
                /* ----------------------------------------------------------------------------------- */

                //Add by Jutarat A. on 25122013
                if (doUpdate.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                    || doUpdate.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                    || doUpdate.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                {
                    if (doUpdate.DummyIDFlag != null && doUpdate.DummyIDFlag.Value == false)
                    {
                        if (CommonUtil.IsNullOrEmpty(doUpdate.IDNo) == false && doUpdate.IDNo.Length != 15)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                            MAS010_Screen,
                                            MessageUtil.MODULE_MASTER,
                                            MessageUtil.MessageList.MSG1060,
                                            null,
                                            new string[] { "cust_IDNo" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                }
                //End Add

                //Check duplicate customer information
                ICustomerMasterHandler cHand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                doUpdate.CustCode = MAS010Param.CustomerData.CustCode;
                bool blnDupIDNo = cHand.CheckDuplicateCustomer_IDNo(doUpdate);
                if (blnDupIDNo)
                {
                    string lblIDno_TaxIDno = CommonUtil.GetLabelFromResource("Master", "MAS010", "lblIDno_TaxIDno");
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS010_Screen,
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1003,
                                        new string[] { lblIDno_TaxIDno },
                                        new string[] { "cust_IDNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else
                {
                    bool blnDupCustNameLC = cHand.CheckDuplicateCustomer_CustNameLC(doUpdate);
                    if (blnDupCustNameLC)
                    {
                        //res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1004);
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        MessageUtil.MessageList msg = MessageUtil.MessageList.MSG1052;
                        if (MAS010_IsCustomerChanged(doUpdate) == true)
                        {
                            if (doUpdate.IsNameLCChanged == true)
                                msg = MessageUtil.MessageList.MSG1004;
                        }

                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, msg);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Update customer data.<br />
        /// - Update customer DB.<br />
        /// - Update customer group DB.<br />
        /// - Update customer site DB.
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public ActionResult MAS010_ConfirmUpdate_Cont(MAS010_UpdateCustomer doUpdate)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                /* --- Customer ---------------------------------------------------------------------- */
                //Set update data to do
                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                MAS010Param.CustomerData.ImportantFlag = doUpdate.ImportantFlag;
                MAS010Param.CustomerData.CustTypeCode = doUpdate.CustTypeCode;
                if (doUpdate.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC)
                {
                    MAS010Param.CustomerData.CompanyTypeCode = doUpdate.CompanyTypeCode;

                    if (doUpdate.CompanyTypeCode == CompanyType.C_COMPANY_TYPE_PUBLIC_CO_LTD)
                        MAS010Param.CustomerData.FinancialMarketTypeCode = doUpdate.FinancialMarketTypeCode;
                }
                MAS010Param.CustomerData.IDNo = doUpdate.IDNo;
                MAS010Param.CustomerData.DummyIDFlag = doUpdate.DummyIDFlag;
                MAS010Param.CustomerData.CustNameEN = doUpdate.CustNameEN;
                MAS010Param.CustomerData.CustFullNameEN = doUpdate.CustFullNameEN;
                MAS010Param.CustomerData.CustNameLC = doUpdate.CustNameLC;
                MAS010Param.CustomerData.CustFullNameLC = doUpdate.CustFullNameLC;
                MAS010Param.CustomerData.RepPersonName = doUpdate.RepPersonName;
                MAS010Param.CustomerData.ContactPersonName = doUpdate.ContactPersonName;
                MAS010Param.CustomerData.SECOMContactPerson = doUpdate.SECOMContactPerson;
                MAS010Param.CustomerData.RegionCode = doUpdate.RegionCode;
                MAS010Param.CustomerData.BusinessTypeCode = doUpdate.BusinessTypeCode;
                MAS010Param.CustomerData.PhoneNo = doUpdate.PhoneNo;
                MAS010Param.CustomerData.FaxNo = doUpdate.FaxNo;
                MAS010Param.CustomerData.AddressEN = doUpdate.AddressEN;
                MAS010Param.CustomerData.AddressLC = doUpdate.AddressLC;
                MAS010Param.CustomerData.AlleyEN = doUpdate.AlleyEN;
                MAS010Param.CustomerData.AlleyLC = doUpdate.AlleyLC;
                MAS010Param.CustomerData.RoadEN = doUpdate.RoadEN;
                MAS010Param.CustomerData.RoadLC = doUpdate.RoadLC;
                MAS010Param.CustomerData.SubDistrictEN = doUpdate.SubDistrictEN;
                MAS010Param.CustomerData.SubDistrictLC = doUpdate.SubDistrictLC;
                MAS010Param.CustomerData.ProvinceCode = doUpdate.ProvinceCodeEN; //either EN or LC is fine, both has same value
                MAS010Param.CustomerData.ProvinceNameEN = doUpdate.ProvinceNameEN;
                MAS010Param.CustomerData.ProvinceNameLC = doUpdate.ProvinceNameLC;
                MAS010Param.CustomerData.DistrictCode = doUpdate.DistrictCodeEN; //either EN or LC is fine, both has same value
                MAS010Param.CustomerData.DistrictNameEN = doUpdate.DistrictNameEN;
                MAS010Param.CustomerData.DistrictNameLC = doUpdate.DistrictNameLC;
                MAS010Param.CustomerData.ZipCode = doUpdate.ZipCode;
                MAS010Param.CustomerData.URL = doUpdate.URL;
                MAS010Param.CustomerData.Memo = doUpdate.Memo;

                //Move to MAS010_ConfirmUpdate
                ////Check system suspending
                //res = checkSystemSuspending();
                //if (res.IsError)
                //    return Json(res);

                //Create full address
                IMasterHandler mHand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                mHand.CreateAddressFull(MAS010Param.CustomerData);

                using (TransactionScope scope = new TransactionScope())
                {
                    // Update customer data
                    ICustomerMasterHandler cHand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                    cHand.UpdateCustomer(MAS010Param.CustomerData);

                    // Update customer group
                    if (MAS010Param.CustomerGruopList_ForView.Count > 0)
                    {
                        cHand.DeleteCustomerGroup(CommonUtil.ConvertToXml_Store(MAS010Param.CustomerGruopList_ForView));
                        cHand.InsertCustomerGroup(MAS010Param.CustomerGruopList_ForView);
                    }
                    else
                    {
                        // delete all
                        mHand.DeleteTbm_CustomerGroup(null, MAS010Param.CustomerCode);
                    }


                    /* --- Site -------------------------------------------------------------------------- */
                    ISiteMasterHandler sHand = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                    foreach (doSite site in MAS010Param.UpdateList)
                    {
                        sHand.UpdateSite(site);
                    }
                    foreach (doSite site in MAS010Param.RemoveList)
                    {
                        sHand.DeleteSite(site);
                    }

                    //clear data in session
                    MAS010Param.CustomerData = new doCustomer();
                    MAS010Param.UpdateList = new List<doSite>();
                    MAS010Param.RemoveList = new List<doSite>();
                    UpdateScreenObject(MAS010Param);

                    scope.Complete();
                }

                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046); //Save Completely
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Validate customer before delete.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS010_ConfirmDelete_P1()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                //Check customer code is used by other system
                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();

                CommonUtil c = new CommonUtil();
                string CustCodeShort = c.ConvertCustCode(MAS010Param.CustomerData.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                bool blnIsUsed = this.isUsedCustomer(MAS010Param.CustomerData.CustCode);
                if (blnIsUsed)
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1059, new string[] { CustCodeShort });
                else
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1005, CustCodeShort);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Delete customer data (logical delete).<br />
        /// - Set delete flag.<br />
        /// - Delete all customer group of this customer.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS010_ConfirmDelete_P2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //res = checkSystemSuspending();
                //if (res.IsError)
                //    return Json(res);

                ////Check customer code is used by other system
                MAS010_ScreenParameter MAS010Param = GetScreenObject<MAS010_ScreenParameter>();
                //bool blnIsUsed = this.isUsedCustomer(MAS010Param.CustomerData.CustCode);

                //if (blnIsUsed)
                //{
                //    CommonUtil c = new CommonUtil();
                //    string CustCodeShort = c.ConvertCustCode(MAS010Param.CustomerData.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1006, new string[] { CustCodeShort });
                //    //res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1006, new string[] { MAS010Param.CustomerData.CustCode });
                //    return Json(res);
                //}
                //else
                //{
                ICustomerMasterHandler cHand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                IMasterHandler handlerMaster = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    MAS010Param.CustomerData.DeleteFlag = true;
                    cHand.UpdateCustomer(MAS010Param.CustomerData);

                    // deleted all customer group
                    handlerMaster.DeleteTbm_CustomerGroup(null, MAS010Param.CustomerCode);

                    scope.Complete();
                }


                //clear data in session
                MAS010Param.CustomerData = new doCustomer();
                MAS010Param.UpdateList = new List<doSite>();
                MAS010Param.RemoveList = new List<doSite>();
                UpdateScreenObject(MAS010Param);

                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0047); //Delete Completely
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        private String MAS010_DisplayFullName(MAS010_ScreenParameter MAS010Param, String CustName, String CompanyTypeCode, bool IsENLang = true)
        {
            string strFullCustName = string.Empty;

            if (!CommonUtil.IsNullOrEmpty(CustName))
            {
                var doCompanyTypeList = from c in MAS010Param.CompanyTypeList
                                        where c.CompanyTypeCode == CompanyTypeCode
                                        select c;

                foreach (var doCompanyType in doCompanyTypeList)
                {
                    string prefix = String.Empty;
                    string suffix = String.Empty;
                    if (IsENLang)
                    {
                        prefix = doCompanyType.CustNamePrefixEN;
                        suffix = doCompanyType.CustNameSuffixEN;
                    }
                    else
                    {
                        prefix = doCompanyType.CustNamePrefixLC;
                        suffix = doCompanyType.CustNameSuffixLC;
                    }

                    prefix = (prefix == null) ? "" : prefix + " ";
                    suffix = (suffix == null) ? "" : " " + suffix;
                    strFullCustName = prefix + CustName + suffix;
                }
            }

            if (strFullCustName == string.Empty)
                strFullCustName = CustName;

            return strFullCustName;
        }

        private bool isUsedSite(String siteCode)
        {
            IQuotationHandler qHand = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            bool bQUIsUsed = qHand.IsUsedSiteData(siteCode);

            IContractHandler cHand = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            bool bCTIsUsed = cHand.IsUsedSiteData(siteCode);

            // Akat K. : if use in quatation or contract it is in use
            //if (bQUIsUsed && bCTIsUsed)
            if (bQUIsUsed || bCTIsUsed)
                return true;
            else
                return false;
        }

        private bool isUsedCustomer(String custCode)
        {
            IQuotationHandler qHand = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            bool bQUIsUsed = qHand.IsUsedCustomerData(custCode);

            IContractHandler cHand = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            bool bCTIsUsed = cHand.IsUsedCustomerData(custCode);

            // Akat K. : if use in quatation or contract it is in use
            //if (bQUIsUsed && bCTIsUsed)
            if (bQUIsUsed || bCTIsUsed)
                return true;
            else
                return false;
        }

        #region Old Code
        //public ActionResult InitialGrid_MAS010()
        //{
        //    return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS010"));
        //}
        //public JsonResult MAS010_GetTbm_Customer()
        //{
        //    try{

        //        IMasterHandler hand = ServiceContainer .GetService<IMasterHandler>() as IMasterHandler;
        //        List<tbm_Customer> lst = hand.GetTbm_Customer(Request["CustomerCode"]);
        //        if (lst.Count > 0)
        //        {
        //            return Json(lst);
        //        }
        //        else
        //        {
        //            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0001);
        //            //return Json(msg);
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public JsonResult MAS010_GetTbm_Site()
        //{
        //    try
        //    {
        //        ObjectResultData res = new ObjectResultData();
        //        List<MessageModel> msgLst = new List<MessageModel>();
        //        ISiteMasterHandler hand = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
        //        List<doGetTbm_Site> lst = hand.GetTbm_Site(Request["CustomerCode"]);
        //        List<View_tbm_Site> lst2 = new List<View_tbm_Site>();
        //        if(lst.Count > 0){
        //        foreach (doGetTbm_Site dt in lst)
        //        {
        //            lst2.Add(CommonUtil.CloneObject<doGetTbm_Site, View_tbm_Site>(dt));
        //        }
        //        string xml = CommonUtil.ConvertToXml<View_tbm_Site>(lst2, "Master\\MAS010");
        //        return Json(xml);
        //        }
        //        else{                   
        //            //msgLst.Add(MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0001));
        //            //res = ResultDataUtil.GetWarningResultData(null, msgLst);
        //            return Json(res);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public ActionResult Check_CustomerCode()
        //{
        //    if (Request["CustomerCode"] == "")
        //    {
        //        try
        //        {
        //            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0006);
        //            //msg.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        //            //return Json(msg);
        //        }
        //        catch (Exception ex)
        //        {

        //           // MessageModel msg = MessageUtil.GetMessage(ex);
        //            //return Json(msg);
        //        }
        //        return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //public ActionResult CheckReqFild_MAS010()
        //{
        //    List<MessageModel> wLst = new List<MessageModel>();
        //    try
        //    {
        //        try
        //        {

        //            ListMandatoryField Man = new ListMandatoryField();
        //            Man.AddRows("SiteNameEN");
        //            Man.AddRows("SiteNameLC");
        //            Man.AddRows("AdressEN2");
        //            Man.AddRows("SoiEN2");
        //            Man.AddRows("RoadEN2");
        //            Man.AddRows("Tambon_KwaengEN2");
        //            Man.AddRows("AdressLC2");
        //            Man.AddRows("SoiLC2");
        //            Man.AddRows("RoadLC2");
        //            Man.AddRows("Tambon_KwaengLC2");

        //            //ApplicationErrorException.CheckMandatoryField(Request, Man, true);
        //        }
        //        catch (Exception ex1)
        //        {
        //            //wLst.Add(MessageUtil.GetMessage(ex1));
        //        }

        //        try
        //        {
        //            ListMandatoryField Man = new ListMandatoryField();
        //            Man.AddRows("UsageCode");
        //            Man.AddRows("ProvinceEN2");
        //            Man.AddRows("DistrictEN2");
        //            Man.AddRows("ProvinceLC2");
        //            Man.AddRows("DistrictLC2");

        //            string[][] nullLst = CommonUtil.CheckMandatoryFiled(Request, Man, true);
        //            if (nullLst != null)
        //            {
        //                string txt_field = string.Empty;
        //                foreach (string f in nullLst[0])
        //                {
        //                    if (txt_field != string.Empty)
        //                        txt_field += ", ";
        //                    txt_field += f;
        //                }

        //                //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0066,
        //                //                                            new string[] { txt_field });

        //                //if (nullLst.Length > 1)
        //                //    msg.Controls = nullLst[1];

        //                //wLst.Add(msg);
        //            }
        //        }
        //        catch (Exception ex2)
        //        {
        //            //wLst.Add(MessageUtil.GetMessage(ex2));
        //        }

        //        if (wLst.Count == 0)
        //            return Json(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //    }

        //    //return Json(MessageUtil.GetWarningMessage(wLst));
        //    return null;
        //}

        //public ActionResult CheckReqFildCust_MAS010()
        //{
        //    List<MessageModel> wLst = new List<MessageModel>();
        //    try
        //    {
        //        try
        //        {

        //            ListMandatoryField Man = new ListMandatoryField();
        //            Man.AddRows("NameEn");
        //            Man.AddRows("NameLc");
        //            Man.AddRows("IDno_TaxIDno");                    
        //            Man.AddRows("AdressEN");
        //            Man.AddRows("SoiEN");
        //            Man.AddRows("RoadEN");
        //            Man.AddRows("Tambon_KwaengEN");
        //            Man.AddRows("AdressLC");
        //            Man.AddRows("SoiLC");
        //            Man.AddRows("RoadLC");
        //            Man.AddRows("Tambon_KwaengLC");

        //            //ApplicationErrorException.CheckMandatoryField(Request, Man, true);
        //        }
        //        catch (Exception ex1)
        //        {
        //            //wLst.Add(MessageUtil.GetMessage(ex1));
        //        }

        //        try
        //        {
        //            ListMandatoryField Man = new ListMandatoryField();
        //            Man.AddRows("CustomerType");
        //            Man.AddRows("Nationality");
        //            Man.AddRows("BusinessType");
        //            Man.AddRows("ProvinceEN");
        //            Man.AddRows("DistrictEN");
        //            Man.AddRows("ProvinceLC");
        //            Man.AddRows("DistrictLC");

        //            string[][] nullLst = CommonUtil.CheckMandatoryFiled(Request, Man, true);
        //            if (nullLst != null)
        //            {
        //                string txt_field = string.Empty;
        //                foreach (string f in nullLst[0])
        //                {
        //                    if (txt_field != string.Empty)
        //                        txt_field += ", ";
        //                    txt_field += f;
        //                }

        //                //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0066,
        //                //                                            new string[] { txt_field });

        //                //if (nullLst.Length > 1)
        //                //    msg.Controls = nullLst[1];

        //                //wLst.Add(msg);
        //            }
        //        }
        //        catch (Exception ex2)
        //        {
        //            //wLst.Add(MessageUtil.GetMessage(ex2));
        //        }

        //        if (wLst.Count == 0)
        //            return Json(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //    }

        //    //return Json(MessageUtil.GetWarningMessage(wLst));
        //    return null;
        //}
        #endregion

        private bool MAS010_IsCustomerChanged(MAS010_UpdateCustomer doUpdate)
        {
            try
            {
                doCustomer custDo = doUpdate as doCustomer;
                doCustomer oCustDo = null;
                MAS010_ScreenParameter custData = GetScreenObject<MAS010_ScreenParameter>();
                if (custData != null)
                {
                    if (custData.CustomerData != null)
                        oCustDo = custData.CustomerData;
                }

                if (oCustDo == null && custDo != null)
                {
                    custDo.IsNameLCChanged = true;
                    return true;
                }
                else if (oCustDo != null && custDo != null)
                {
                    custDo.IsNameLCChanged = false;
                    if (oCustDo.CustNameLC != custDo.CustNameLC)
                        custDo.IsNameLCChanged = true;

                    bool isSame = true;
                    List<string> chkPropLst = new List<string>() 
                    { 
                        "CustCode",
                        "CustTypeCode", 
                        "CompanyTypeCode", 
                        "FinancialMarketTypeCode", 
                        "IDNo",
                        "CustNameEN",
                        "CustFullNameEN",
                        "CustNameLC",
                        "CustFullNameLC",
                        "RepPersonName",
                        "ContactPersonName",
                        "SECOMContactPerson",
                        "RegionCode",
                        "BusinessTypeCode",
                        "PhoneNo",
                        "FaxNo",
                        "URL",
                        "AddressEN",
                        "AlleyEN",
                        "RoadEN",
                        "SubDistrictEN",
                        "ProvinceEN",
                        "DistrictEN",
                        "AddressLC",
                        "AlleyLC",
                        "RoadLC",
                        "SubDistrictLC",
                        "ProvinceLC",
                        "DistrictLC",
                        "Zipcode"
                    };
                    PropertyInfo[] props = typeof(doCustomer).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.CanWrite == false)
                            continue;
                        if (chkPropLst.IndexOf(prop.Name) < 0)
                            continue;

                        object obj1 = prop.GetValue(oCustDo, null);
                        object obj2 = prop.GetValue(custDo, null);

                        if (CommonUtil.IsNullOrEmpty(obj1) == false || CommonUtil.IsNullOrEmpty(obj2) == false)
                        {
                            isSame = false;
                            if (CommonUtil.IsNullOrEmpty(obj1) == false && CommonUtil.IsNullOrEmpty(obj2) == false)
                            {
                                if (obj1.ToString() == obj2.ToString())
                                    isSame = true;
                            }

                            if (isSame == false)
                                break;
                        }
                    }

                    if (isSame == false)
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get SpecialCareFlag from tbm_BuildingUsageList in ScreenParameter that match to UsageCode
        /// </summary>
        /// <param name="UsageCode"></param>
        /// <returns></returns>
        public ActionResult MAS010_GetAttachImportanceFlag(string UsageCode)
        {
            IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            MAS010_ScreenParameter screenParam = GetScreenObject<MAS010_ScreenParameter>();
            if (CommonUtil.IsNullOrEmpty(UsageCode) == false)
            {
                //List<tbm_BuildingUsage> ulst = mhandler.GetTbm_BiuldingUsage();
                if (screenParam.tbm_BuildingUsageList.Count > 0)
                {
                    foreach (tbm_BuildingUsage u in screenParam.tbm_BuildingUsageList)
                    {
                        if (UsageCode == u.BuildingUsageCode)
                        {
                            res.ResultData = u.SpecialCareFlag;
                            break;
                        }
                    }

                }
            }
            return Json(res);
        }

    }

}
