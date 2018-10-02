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
using SECOM_AJIS.Presentation.Master.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System.Reflection;



namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS030_Screen = "MAS030";

        #region Authority

        /// <summary>
        /// - Check permission for screen MAS030.<br />
        /// - Check system suspending.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS030_Authority(MAS030_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Parameter is OK ?
                if (param.InputData == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS030_ScreenParameter>("MAS030", param, res);
        }
        #endregion

        #region Views

        /// <summary>
        /// Initial screen and get company type list.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS030_Screen)]
        public ActionResult MAS030()
        {

            List<doCompanyType> companyTypeList = new List<doCompanyType>();
            doBillingClient MAS030Input = new doBillingClient();
            MAS030_ScreenParameter MAS030Param = new MAS030_ScreenParameter();

            ViewBag.mas030_ValidateBillingClient = false;

            try
            {
                MAS030Param = GetScreenObject<MAS030_ScreenParameter>();
                if (MAS030Param.InputData != null)
                {
                    MAS030Input = MAS030Param.InputData;

                    string[] chkVal = new string[]
                    {
                        "CustTypeCode",
                        "CompanyTypeCode",
                        "BusinessTypeCode",
                        "PhoneNo",
                        "IDNo",
                        "RegionCode",
                        "AddressEN",
                        "AddressLC"
                    };
                    foreach (string val in chkVal)
                    {
                        PropertyInfo prop = MAS030Input.GetType().GetProperty(val);
                        if (prop != null)
                        {
                            if (CommonUtil.IsNullOrEmpty(prop.GetValue(MAS030Input, null)) == false)
                            {
                                MAS030Input.ValidateBillingClient = true;
                                break;
                            }
                        }
                    }
                }

                ICustomerMasterHandler custHand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                companyTypeList = custHand.GetCompanyType(null); //Get All Company Type
                MAS030Param.CompanyTypeList = companyTypeList;

                if (MAS030Input != null)
                {
                    ViewBag.mas030_BillingClientCode = MAS030Input.BillingClientCode ?? "";
                    ViewBag.mas030_CustomerType = MAS030Input.CustTypeCode ?? "";
                    ViewBag.mas030_CompanyType = MAS030Input.CompanyTypeCode ?? "";
                    ViewBag.mas030_IDNo = MAS030Input.IDNo ?? "";
                    ViewBag.mas030_CustNameEN = MAS030Input.NameEN ?? "";
                    ViewBag.mas030_CustNameLC = MAS030Input.NameLC ?? "";
                    ViewBag.mas030_BranchNo = MAS030Input.BranchNo ?? "";
                    ViewBag.mas030_BranchNameEN = MAS030Input.BranchNameEN ?? "";
                    ViewBag.mas030_BranchNameLC = MAS030Input.BranchNameLC ?? "";
                    ViewBag.mas030_AddressEN = MAS030Input.AddressEN ?? "";
                    ViewBag.mas030_AddressLC = MAS030Input.AddressLC ?? "";
                    ViewBag.mas030_Nationality = MAS030Input.RegionCode ?? "";
                    ViewBag.mas030_BusinessType = MAS030Input.BusinessTypeCode ?? "";
                    ViewBag.mas030_TelephoneNo = MAS030Input.PhoneNo ?? "";
                    ViewBag.mas030_CustFullNameEN = MAS030_DisplayFullName(MAS030Param, MAS030Input.NameEN, MAS030Input.CompanyTypeCode, true);
                    ViewBag.mas030_CustFullNameLC = MAS030_DisplayFullName(MAS030Param, MAS030Input.NameLC, MAS030Input.CompanyTypeCode, false);
                    ViewBag.mas030_ValidateBillingClient = MAS030Input.ValidateBillingClient;
                
                }
               
                ViewBag.mas030_CustTypeJuristic = CustomerType.C_CUST_TYPE_JURISTIC;

                ViewBag.HeadOfficeEN = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_EN).ToUpper(); //Add by Jutarat A. on 16122013
                ViewBag.HeadOfficeLC = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_LC); //Add by Jutarat A. on 16122013

                UpdateScreenObject(MAS030Param); //keep companyTypeCodeList in session
            }
            catch
            {

            }



            return View(MAS030_Screen);
        }
        #endregion

        /// <summary>
        /// Generate company full name for display.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS030_CompanyChange(MAS030_GetCompanyFullName param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS030_ScreenParameter MAS030Param = GetScreenObject<MAS030_ScreenParameter>();
                param.FullNameEN = MAS030_DisplayFullName(MAS030Param, param.NameEN, param.CompanyTypeCode, true);
                param.FullNameLC = MAS030_DisplayFullName(MAS030Param, param.NameLC, param.CompanyTypeCode, false);

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
        public ActionResult MAS030_CustNameENChange(MAS030_GetCompanyFullName param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS030_ScreenParameter MAS030Param = GetScreenObject<MAS030_ScreenParameter>();
                param.FullNameEN = MAS030_DisplayFullName(MAS030Param, param.NameEN, param.CompanyTypeCode, true);
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
        public ActionResult MAS030_CustNameLCChange(MAS030_GetCompanyFullName param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS030_ScreenParameter MAS030Param = GetScreenObject<MAS030_ScreenParameter>();
                param.FullNameLC = MAS030_DisplayFullName(MAS030Param, param.NameLC, param.CompanyTypeCode, false);
                res.ResultData = param;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Check is user input all require field.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS030_CheckReqField(MAS030_CheckReqField param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }


                //Add by Jutarat A. on 25122013
                if (param.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                    || param.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                    || param.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                {
                    ValidatorUtil validator = new ValidatorUtil();

                    if (CommonUtil.IsNullOrEmpty(param.IDNo))
                        validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                                  "MAS030",
                                                  MessageUtil.MODULE_COMMON,
                                                  MessageUtil.MessageList.MSG0007,
                                                  "mas030_IDNo",
                                                  "lblIDnoTaxIDno",
                                                  "mas030_IDNo");

                    //if (CommonUtil.IsNullOrEmpty(param.BranchNo))
                    //    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                    //                              "MAS030",
                    //                              MessageUtil.MODULE_COMMON,
                    //                              MessageUtil.MessageList.MSG0007,
                    //                              "mas030_BranchNo",
                    //                              "lblBranchNo",
                    //                              "mas030_BranchNo");

                    //if (CommonUtil.IsNullOrEmpty(param.BranchNameEN))
                    //    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                    //                              "MAS030",
                    //                              MessageUtil.MODULE_COMMON,
                    //                              MessageUtil.MessageList.MSG0007,
                    //                              "mas030_BranchNameEN",
                    //                              "lblBranchNameEn",
                    //                              "mas030_BranchNameEN");

                    //if (CommonUtil.IsNullOrEmpty(param.BranchNameLC))
                    //    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                    //                              "MAS030",
                    //                              MessageUtil.MODULE_COMMON,
                    //                              MessageUtil.MessageList.MSG0007,
                    //                              "mas030_BranchNameLC",
                    //                              "lblBranchNameLc",
                    //                              "mas030_BranchNameLC");

                    ValidatorUtil.BuildErrorMessage(res, validator);
                    if (res.IsError)
                        return Json(res);
                }

                if (param.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                    || param.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                    || param.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                {
                    if (CommonUtil.IsNullOrEmpty(param.IDNo) == false && param.IDNo.Length != 15)
                    {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                            "MAS030",
                                            MessageUtil.MODULE_MASTER,
                                            MessageUtil.MessageList.MSG1060,
                                            null,
                                            new string[] { "mas030_IDNo" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                    }

                    if (CommonUtil.IsNullOrEmpty(param.BranchNo) == false && param.BranchNo.Length != 5)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        "MAS030",
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1061,
                                        null,
                                        new string[] { "mas030_BranchNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    if (param.BranchType == "2" && param.BranchNo == "00000")
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        "MAS030",
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1062,
                                        null,
                                        new string[] { "mas030_BranchNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                //End Add


                bool bResult = true;

                //if data from control <> data in doBillingClient (from initial) then set billingClientCode = empty
                MAS030_ScreenParameter MAS030Param = GetScreenObject<MAS030_ScreenParameter>();
                doBillingClient inputDO = MAS030Param.InputData;

                if (CommonUtil.IsNullOrEmpty(inputDO.BillingClientCode))
                {
                    bResult = false;

                }
                else if ((inputDO.CustTypeCode != param.CustTypeCode) ||
                        (inputDO.CompanyTypeCode != param.CompanyTypeCode) ||
                        (inputDO.IDNo != param.IDNo) ||
                        (inputDO.NameEN != param.NameEN) ||
                        (inputDO.NameLC != param.NameLC) ||
                        //(inputDO.BranchNameEN != param.BranchNameEN) ||
                        //(inputDO.BranchNameLC != param.BranchNameLC) ||
                        (inputDO.AddressEN != param.AddressEN) ||
                        (inputDO.AddressLC != param.AddressLC) ||
                        (inputDO.RegionCode != param.RegionCode) ||
                        (inputDO.BusinessTypeCode != param.BusinessTypeCode) ||
                        (inputDO.PhoneNo != param.PhoneNo))
                {
                    bResult = false;
                }

                res.ResultData = bResult;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        private String MAS030_DisplayFullName(MAS030_ScreenParameter MAS030Param, String CustName, String CompanyTypeCode, bool IsENLang = true)
        {
            string strFullCustName = string.Empty;

            if (!CommonUtil.IsNullOrEmpty(CustName))
            {
                var doCompanyTypeList = from c in MAS030Param.CompanyTypeList
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

        /// <summary>
        /// Get English name of Billing match to cond for using in auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult GetNameEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listCustNameEN = new List<string>();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doBillingNameEN> lst = hand.GetBillingNameEn(cond);

                foreach (var item in lst)
                {
                    listCustNameEN.Add(item.NameEN);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listCustNameEN.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Get local name of Billing match to cond for using in auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult GetNameLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listCustNameLC = new List<string>();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doGetBillingNameLC> lst = hand.GetBillingNameLC(cond);

                foreach (var item in lst)
                {
                    listCustNameLC.Add(item.NameLC);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listCustNameLC.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Get English name of Branch match to cond for using in auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult GetBranchNameEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listBranchNameEN = new List<string>();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doBillingBranchNameEN> lst = hand.GetBillingBranchNameEN(cond);

                foreach (var item in lst)
                {
                    listBranchNameEN.Add(item.BranchNameEN);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listBranchNameEN.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Get local name of Branch match to cond for using in auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult GetBranchNameLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listBranchNameLC = new List<string>();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doBillingBranchNameLC> lst = hand.GetBillingBranchNameLC(cond);

                foreach (var item in lst)
                {
                    listBranchNameLC.Add(item.BranchNameLC);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listBranchNameLC.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Get English billing address match to cond for using in auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult GetAddressEN(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listAddressEN = new List<string>();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doBillingAddressNameEN> lst = hand.GetBillingAddressNameEN(cond);

                foreach (var item in lst)
                {
                    listAddressEN.Add(item.AddressEN);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            res.ResultData = listAddressEN.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Get local billing address match to cond for using in auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult GetAddressLC(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listAddressLC = new List<string>();
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doBillingAddressNameLC> lst = hand.GetBillingAddressNameLC(cond);

                foreach (var item in lst)
                {
                    listAddressLC.Add(item.AddressLC);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            res.ResultData = listAddressLC.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Validate user input.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS030_ValidateData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                doBillingClient custDo = null;

                MAS030_ScreenParameter custData = GetScreenObject<MAS030_ScreenParameter>();
                if (custData != null)
                {
                    if (custData.InputData != null)
                        custDo = custData.InputData;
                }

                if (custDo != null)
                {
                    //IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                    #region Set Misc Data

                    MiscTypeMappingList miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(custDo);
                    chandler.MiscTypeMappingList(miscList);

                    #endregion
                }

                MAS030_ValidateCombo validate = CommonUtil.CloneObject<doBillingClient, MAS030_ValidateCombo>(custDo);
                ValidatorUtil.BuildErrorMessage(res, new object[] { validate });

                if (custDo != null)
                {
                    if (custDo.ValidateBillingClient == true)
                    {
                        MAS030_CheckReqField vq = CommonUtil.CloneObject<doBillingClient, MAS030_CheckReqField>(custDo);
                        ValidatorUtil.BuildErrorMessage(res, new object[] { vq });
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #region Old Code
        //[HttpPost]
        //public ActionResult GetBranchNameEN(string cond)
        //{
        //    try
        //    {
        //        IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
        //        List<doBillingBranchNameEN> lst = hand.GetBillingBranchNameEN(cond);
        //        string xml = CommonUtil.ConvertToXml<doBillingBranchNameEN>(lst);
        //        return Json(xml);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //[HttpPost]
        //public ActionResult GetBranchNameLC(string cond)
        //{
        //    try
        //    {
        //        IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
        //        List<doBillingBranchNameLC> lst = hand.GetBillingBranchNameLC(cond);
        //        string xml = CommonUtil.ConvertToXml<doBillingBranchNameLC>(lst);
        //        return Json(xml);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //[HttpPost]
        //public ActionResult GetAddressEN(string cond)
        //{
        //    try
        //    {
        //        IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
        //        List<doBillingAddressNameEN> lst = hand.GetBillingAddressNameEN(cond);
        //        string xml = CommonUtil.ConvertToXml<doBillingAddressNameEN>(lst);
        //        return Json(xml);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //[HttpPost]
        //public ActionResult GetAddressLC(string cond)
        //{
        //    try
        //    {
        //        IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
        //        List<doBillingAddressNameLC> lst = hand.GetBillingAddressNameLC(cond);
        //        string xml = CommonUtil.ConvertToXml<doBillingAddressNameLC>(lst);
        //        return Json(xml);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //[HttpPost]
        //public ActionResult CheckReqFild_MAS030()
        //{
        //    List<MessageModel> wLst = new List<MessageModel>();
        //    try
        //    {
        //        try
        //        {
        //            ListMandatoryField Man = new ListMandatoryField();

        //            Man.AddRows("NameEn");
        //            Man.AddRows("NameLc");
        //            Man.AddRows("AddressEn");                 
        //            Man.AddRows("AddressLc");


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
        //            string[][] nullLst = CommonUtil.CheckMandatoryFiled(Request, Man, true);

        //            string txt_field = string.Empty;
        //            foreach (string f in nullLst[0])
        //            {
        //                if (txt_field != string.Empty)
        //                    txt_field += ", ";
        //                txt_field += f;
        //            }

        //            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0066,
        //            //                                            new string[] { txt_field });

        //            //if (nullLst.Length > 1)
        //            //    msg.Controls = nullLst[1];

        //            //wLst.Add(msg);
        //        }
        //        catch (Exception ex2)
        //        {
        //            //wLst.Add(MessageUtil.GetMessage(ex2));
        //        }

        //        if (wLst.Count == 0)
        //            return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //    //return Json(MessageUtil.GetWarningMessage(wLst));
        //    return null;
        //}
        #endregion
    }
}