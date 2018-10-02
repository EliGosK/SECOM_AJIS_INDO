//*********************************
// Create by: Attawhit  Chuoosathan
// Create date: 04/July/2010
// Update date: 04/July/2010
//*********************************

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Master.Models;
using System.Reflection;



namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS050_Screen = "MAS050";

        #region Authority

        /// <summary>
        /// Go to View.
        /// </summary>
        /// <param name="custData"></param>
        /// <returns></returns>
        public ActionResult MAS050_Authority(MAS050_ScreenParameter custData)
        {
            //MAS050_ScreenParameter custData = new MAS050_ScreenParameter()
            //{
            //    doCustomer = custDo
            //};

            return InitialScreenEnvironment<MAS050_ScreenParameter>(MAS050_Screen, custData);
        }

        #endregion
        #region Views

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS050_Screen)]
        public ActionResult MAS050()
        {
            try
            {
                ViewBag.IsFullValidate = true;
                ViewBag.IsDummyFlag = true;

                MAS050_ScreenParameter custData = GetScreenObject<MAS050_ScreenParameter>();
                if (custData != null)
                {
                    if (custData.CallerScreenID == ScreenID.C_SCREEN_ID_PROJ_NEW
                        || custData.CallerScreenID == ScreenID.C_SCREEN_ID_PROJ_CHANGE
                        || custData.CallerScreenID == ScreenID.C_SCREEN_ID_QTN_TARGET)
                    {
                        ViewBag.IsFullValidate = false;
                    }

                    doCustomer custDo = custData.doCustomer;
                    if (custDo != null)
                    {
                        ViewBag.CustCode = custDo.CustCodeShort;
                        ViewBag.CustStatus = custDo.CustStatusCodeName;
                        ViewBag.CustomerType = custDo.CustTypeCode;
                        ViewBag.CompanyType = custDo.CompanyTypeCode;
                        ViewBag.FinancialMarketType = custDo.FinancialMarketTypeCode;
                        ViewBag.IDNo = custDo.IDNo;
                        ViewBag.IsDummyFlag = custDo.DummyIDFlag == true ? true : false;
                        ViewBag.CustNameEN = custDo.CustNameEN;
                        ViewBag.CustNameLC = custDo.CustNameLC;
                        ViewBag.RepPersonName = custDo.RepPersonName;
                        ViewBag.ContactPersonName = custDo.ContactPersonName;
                        ViewBag.SECOMContactPerson = custDo.SECOMContactPerson;
                        ViewBag.Nationality = custDo.RegionCode;
                        ViewBag.BusinessType = custDo.BusinessTypeCode;
                        ViewBag.PhoneNo = custDo.PhoneNo;
                        ViewBag.FaxNo = custDo.FaxNo;
                        ViewBag.URL = custDo.URL;
                        ViewBag.AddressEN = custDo.AddressEN;
                        ViewBag.AddressLC = custDo.AddressLC;
                        ViewBag.AlleyEN = custDo.AlleyEN;
                        ViewBag.AlleyLC = custDo.AlleyLC;
                        ViewBag.RoadEN = custDo.RoadEN;
                        ViewBag.RoadLC = custDo.RoadLC;
                        ViewBag.SubDistrictEN = custDo.SubDistrictEN;
                        ViewBag.SubDistrictLC = custDo.SubDistrictLC;
                        ViewBag.ProvinceEN = custDo.ProvinceCode;
                        ViewBag.ProvinceLC = custDo.ProvinceCode;
                        ViewBag.DistrictEN = custDo.DistrictCode;
                        ViewBag.DistrictLC = custDo.DistrictCode;
                        ViewBag.ZipCode = custDo.ZipCode;
                    }
                }

            }
            catch (Exception)
            {
            }

            return View();
        }

        #endregion
        #region Actions

        /// <summary>
        /// Validate customer data.<br />
        /// - Set company type.<br />
        /// - Set nationality.<br />
        /// - Set business type.<br />
        /// - Set province.<br />
        /// - Set district.<br />
        /// - Check require field.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS050_ValidateData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                doCustomer custDo = null;

                MAS050_ScreenParameter custData = GetScreenObject<MAS050_ScreenParameter>();
                if (custData != null)
                {
                    if (custData.doCustomer != null)
                        custDo = custData.doCustomer;
                }

                if (custDo != null)
                {
                    IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                    #region Set Misc Data

                    MiscTypeMappingList miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(custDo);
                    chandler.MiscTypeMappingList(miscList);

                    #endregion
                    #region Company Type

                    if (CommonUtil.IsNullOrEmpty(custDo.CompanyTypeCode) == false)
                    {
                        List<doCompanyType> clst = mhandler.GetCompanyType(custDo.CompanyTypeCode);
                        if (clst.Count > 0)
                            custDo.CompanyTypeName = clst[0].CompanyTypeName;
                    }

                    #endregion
                    #region Nationality Data

                    if (CommonUtil.IsNullOrEmpty(custDo.RegionCode) == false)
                    {
                        List<tbm_Region> nlst = mhandler.GetTbm_Region();
                        if (nlst.Count > 0)
                        {
                            foreach (tbm_Region r in nlst)
                            {
                                if (custDo.RegionCode == r.RegionCode)
                                {
                                    custDo.Nationality = r.Nationality;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                    #region BusinessType Data

                    if (CommonUtil.IsNullOrEmpty(custDo.BusinessTypeCode) == false)
                    {
                        List<tbm_BusinessType> blst = mhandler.GetTbm_BusinessType();
                        if (blst.Count > 0)
                        {
                            foreach (tbm_BusinessType b in blst)
                            {
                                if (custDo.BusinessTypeCode == b.BusinessTypeCode)
                                {
                                    custDo.BusinessTypeName = b.BusinessTypeName;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                    #region Province Data

                    if (CommonUtil.IsNullOrEmpty(custDo.ProvinceCode) == false)
                    {
                        List<tbm_Province> plst = mhandler.GetTbm_Province();
                        if (plst.Count > 0)
                        {
                            foreach (tbm_Province pv in plst)
                            {
                                if (custDo.ProvinceCode == pv.ProvinceCode)
                                {
                                    custDo.ProvinceNameEN = pv.ProvinceNameEN;
                                    custDo.ProvinceNameLC = pv.ProvinceNameLC;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                    #region District

                    if (CommonUtil.IsNullOrEmpty(custDo.DistrictCode) == false)
                    {
                        List<tbm_District> dlst = mhandler.GetTbm_District(custDo.ProvinceCode);
                        if (dlst.Count > 0)
                        {
                            foreach (tbm_District d in dlst)
                            {
                                if (custDo.ProvinceCode == d.ProvinceCode
                                    && custDo.DistrictCode == d.DistrictCode)
                                {
                                    custDo.DistrictNameEN = d.DistrictNameEN;
                                    custDo.DistrictNameLC = d.DistrictNameLC;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    if (CommonUtil.IsNullOrEmpty(custDo.CustTypeName) || custDo.CustTypeCode != CustomerType.C_CUST_TYPE_JURISTIC)
                        custDo.CompanyTypeName = null;
                    if (CommonUtil.IsNullOrEmpty(custDo.CompanyTypeName) || custDo.CompanyTypeCode != CompanyType.C_COMPANY_TYPE_PUBLIC_CO_LTD)
                        custDo.FinancialMaketTypeName = null;
                }

                MAS050_ValidateCombo validate = CommonUtil.CloneObject<doCustomer, MAS050_ValidateCombo>(custDo);
                ValidatorUtil.BuildErrorMessage(res, new object[] { validate });

                if (custDo != null)
                {
                    if (custDo.ValidateCustomerData == false)
                    {

                        object validate1 = null;
                        if (custData.CallerScreenID == ScreenID.C_SCREEN_ID_PROJ_NEW
                        || custData.CallerScreenID == ScreenID.C_SCREEN_ID_PROJ_CHANGE
                        || custData.CallerScreenID == ScreenID.C_SCREEN_ID_QTN_TARGET)
                        {
                            if (CommonUtil.IsNullOrEmpty(custDo.CustCode) == true)
                            {
                                MAS050_CheckRequiredFieldCustNull cCustDo = CommonUtil.CloneObject<doCustomer, MAS050_CheckRequiredFieldCustNull>(custDo);
                                validate1 = cCustDo;
                            }
                            else
                            {
                                MAS050_CheckRequiredFieldNotFull cCustDo = CommonUtil.CloneObject<doCustomer, MAS050_CheckRequiredFieldNotFull>(custDo);
                                validate1 = cCustDo;
                            }
                        }
                        else
                        {
                            MAS050_CheckRequiredField cCustDo = CommonUtil.CloneObject<doCustomer, MAS050_CheckRequiredField>(custDo);
                            validate1 = cCustDo;
                        }

                        ValidatorUtil.BuildErrorMessage(res, new object[] { validate1 });
                    }
                }

                //Add by Jutarat A. on 02012014
                if (custDo != null)
                {
                    if (custDo.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                        || custDo.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                        || custDo.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                    {
                        if (custDo.DummyIDFlag != null && custDo.DummyIDFlag.Value == false)
                        {
                            if (CommonUtil.IsNullOrEmpty(custDo.IDNo) == false && custDo.IDNo.Length != 15)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                                "MAS050",
                                                MessageUtil.MODULE_MASTER,
                                                MessageUtil.MessageList.MSG1060,
                                                null,
                                                new string[] { "IDNo" });
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                return Json(res);
                            }
                        }
                    }
                }
                //End Add
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial Customer Group table.<br />
        /// Send Customer Group data to show in table.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS050_InitialGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<dtCustomeGroupData> lst = new List<dtCustomeGroupData>();

                MAS050_ScreenParameter custData = GetScreenObject<MAS050_ScreenParameter>();
                if (custData != null)
                {
                    if (custData.doCustomer != null)
                        lst = custData.doCustomer.CustomerGroupData;
                }
                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(lst, "Master\\MAS050", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate customer group before add to table.<br />
        /// - Check is it already in table.<br />
        /// - Check is it exist Customer Group DB/<br />
        /// </summary>
        /// <param name="custGroup"></param>
        /// <returns></returns>
        public ActionResult MAS050_CheckBeforeAddCustomerGroup(MAS050_CustomerGroup custGroup)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                #region Check Duplicate

                if (custGroup.CustomerGroupList != null)
                {
                    foreach (dtCustomerGroup cg in custGroup.CustomerGroupList)
                    {
                        if (cg.GroupCode == custGroup.GroupCode)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1007);
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                }

                #endregion
                #region Check Duplicate Customer Group

                ICustomerMasterHandler hand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                List<int?> list = hand.CheckExistCustomerGroup(
                                        custGroup.GroupCode,
                                        custGroup.CustCode);
                if (list.Count > 0)
                {
                    if (list[0] > 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1007);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }

                #endregion

                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<View_tbm_Group> lstGroup = handler.GetDorp_Group();
                if (lstGroup.Count > 0)
                {
                    foreach (View_tbm_Group g in lstGroup)
                    {
                        if (g.GroupCode == custGroup.GroupCode)
                        {
                            res.ResultData = g;
                            break;
                        }
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
        /// Get company type data to show in screen.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult MAS050_GetCompanyData(tbm_CompanyType cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.IsNullOrEmpty(cond.CompanyTypeCode) == false)
                {
                    ICustomerMasterHandler hand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                    List<doCompanyType> list = hand.GetCompanyType(cond.CompanyTypeCode);
                    if (list.Count > 0)
                        res.ResultData = list[0];
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate customer data.<br />
        /// - Check require field.<br />
        /// - Check duplicate ID.<br />
        /// - Check duplicate local name.<br />
        /// - Generate full address.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public ActionResult MAS050_ConfirmData(MAS050_CheckRequiredField customer)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    if (customer.IsFullValidate)
                    {
                        ValidatorUtil.BuildErrorMessage(res, this);
                        return Json(res);
                    }
                    else
                    {
                        object validate = null;
                        if (CommonUtil.IsNullOrEmpty(customer.CustCode) == true)
                            validate = CommonUtil.CloneObject<doCustomer, MAS050_CheckRequiredFieldCustNull>(customer);
                        else
                            validate = CommonUtil.CloneObject<doCustomer, MAS050_CheckRequiredFieldNotFull>(customer);

                        ValidatorUtil.BuildErrorMessage(res, new object[] { validate });
                        if (res.IsError)
                            return Json(res);
                    }
                }

                //Add by Jutarat A. on 02012014
                if (customer != null)
                {
                    if (customer.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                        || customer.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                        || customer.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                    {
                        if (customer.DummyIDFlag != null && customer.DummyIDFlag.Value == false)
                        {
                            if (CommonUtil.IsNullOrEmpty(customer.IDNo) == false && customer.IDNo.Length != 15)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                                "MAS050",
                                                MessageUtil.MODULE_MASTER,
                                                MessageUtil.MessageList.MSG1060,
                                                null,
                                                new string[] { "IDNo" });
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                return Json(res);
                            }
                        }
                    }
                }
                //End Add

                #region Update code

                CommonUtil cmm = new CommonUtil();
                if (customer != null)
                    customer.CustCode = cmm.ConvertCustCode(customer.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                #endregion

                string callerID = null;
                MAS050_ScreenParameter param = GetScreenObject<MAS050_ScreenParameter>();
                if (param != null)
                {
                    callerID = param.CallerScreenID;
                }

                if (callerID != ScreenID.C_SCREEN_ID_QTN_TARGET)
                {
                    bool isChanged = MAS050_IsCustomerChanged(customer);
                    if (isChanged == true)
                    {
                        customer.CustCode = null;
                        customer.CustStatus = null;
                        customer.CustStatusName = null;

                        ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                        #region Check duplicate customer

                        if (customer != null)
                        {
                            if (custhandler.CheckDuplicateCustomer_IDNo(customer) == true)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                return Json(res);
                            }
                            if (custhandler.CheckDuplicateCustomer_CustNameLC(customer) == true)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1004);
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                            }
                        }

                        #endregion
                    }
                }

                #region Create Customer Address Full

                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                mhandler.CreateAddressFull(customer);

                #endregion
                res.ResultData = customer;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Methods

        private bool MAS050_IsCustomerChanged(doCustomerWithGroup custDo)
        {
            try
            {
                doCustomerWithGroup oCustDo = null;
                MAS050_ScreenParameter custData = GetScreenObject<MAS050_ScreenParameter>();
                if (custData != null)
                {
                    if (custData.doCustomer != null)
                        oCustDo = custData.doCustomer;
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
                        "CustomerType", 
                        "CompanyType", 
                        "FinancialMarketType", 
                        "IDNo",
                        "CustNameEN",
                        "CustFullNameEN",
                        "CustNameLC",
                        "CustFullNameLC",
                        "RepPersonName",
                        "ContactPersonName",
                        "SECOMContactPerson",
                        "Nationality",
                        "BusinessType",
                        "TelePhoneNo",
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
                        "Zipcode",
                        "CustomerGroupData"
                    };
                    PropertyInfo[] props = typeof(doCustomerWithGroup).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.CanWrite == false)
                            continue;
                        if (chkPropLst.IndexOf(prop.Name) < 0)
                            continue;

                        if (prop.Name == "CustomerGroupData")
                        {
                            if (oCustDo.CustomerGroupData != null
                                && custDo.CustomerGroupData != null)
                            {
                                if (oCustDo.CustomerGroupData.Count == custDo.CustomerGroupData.Count)
                                {
                                    foreach (dtCustomeGroupData gc in oCustDo.CustomerGroupData)
                                    {
                                        bool sameGroup = false;
                                        foreach (dtCustomeGroupData gr in custDo.CustomerGroupData)
                                        {
                                            if (gc.GroupCode == gr.GroupCode)
                                            {
                                                sameGroup = true;
                                                break;
                                            }
                                        }
                                        if (sameGroup == false)
                                        {
                                            isSame = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
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

        #endregion
    }
}