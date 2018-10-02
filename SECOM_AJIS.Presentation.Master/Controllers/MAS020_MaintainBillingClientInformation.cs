//*********************************
// Create by: Fikree S.
// Create date: 13/Jan/2012
// Update date: 13/Jan/2012
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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of MAS020
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS020_Authority(MAS020_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                

                return InitialScreenEnvironment<MAS020_ScreenParameter>("MAS020", param ,res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen MAS020 and get company type data
        /// </summary>
        /// <returns></returns>
        [Initialize("MAS020")]
        public ActionResult MAS020()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ViewBag.HasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.HasPermissionDelete = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_DEL);

                ViewBag.HeadOfficeEN = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_EN).ToUpper(); //Add by Jutarat A. on 16122013
                ViewBag.HeadOfficeLC = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_LC); //Add by Jutarat A. on 16122013

                ICustomerMasterHandler masterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                List<doCompanyType> companyTypeList = masterHandler.GetCompanyType(null);
                MAS020_ScreenParameter screenParam = GetScreenObject<MAS020_ScreenParameter>();
                screenParam.CompanyTypeList = companyTypeList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return View();
        }

        /// <summary>
        /// Retrieve billing client data
        /// </summary>
        /// <param name="BillingClientCodeSearch"></param>
        /// <returns></returns>
        public ActionResult MAS020_Retrieve(string BillingClientCodeSearch)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (CommonUtil.IsNullOrEmpty(BillingClientCodeSearch))
                {
                    string lblBillingClientCode = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblBillingClientCode");
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        "MAS020",
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1045,
                                        new string[] { lblBillingClientCode },
                                        new string[] { "BillingClientCodeSearch" });
                    return Json(res);
                }
                else
                {
                    CommonUtil cm = new CommonUtil();
                    string BillingClientCode_LongFormat = cm.ConvertBillingClientCode(BillingClientCodeSearch, CommonUtil.CONVERT_TYPE.TO_LONG);

                    IBillingClientMasterHandler hand = ServiceContainer.GetService<IBillingClientMasterHandler>() as IBillingClientMasterHandler;
                    List<tbm_BillingClient> list = hand.GetTbm_BillingClient(BillingClientCode_LongFormat);

                    tbm_BillingClient data = null;
                    if (list.Count > 0)
                    {
                        data = list[0];
                        if (data.DeleteFlag == true)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        "MAS020",
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1058,
                                        null,
                                        new string[] { "BillingClientCodeSearch" });
                            return Json(res);
                        }
                        MAS020_ScreenParameter MAS020Param = GetScreenObject<MAS020_ScreenParameter>();
                        MAS020Param.currentBillingClient = data;

                    }
                    else 
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        "MAS020",
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1058,
                                        null,
                                        new string[] { "BillingClientCodeSearch" });
                    }
                    res.ResultData = data;
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Update billing client data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS020_Update(MAS020_BillingClientData  data)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                data.DeleteFlag = (data.DeleteFlag == null ? false : true);

                if (!(bool)data.DeleteFlag)
                {
                    // Check required field.
                    if (ModelState.IsValid == false)
                    {
                        ValidatorUtil.BuildErrorMessage(res, this);
                        if (res.IsError)
                            return Json(res);
                    }

                    //Add by Jutarat A. on 25122013
                    if (data.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                        || data.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                        || data.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                    {
                        ValidatorUtil validator = new ValidatorUtil();

                        if (CommonUtil.IsNullOrEmpty(data.IDNo))
                            validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                                      "MAS020",
                                                      MessageUtil.MODULE_COMMON,
                                                      MessageUtil.MessageList.MSG0007,
                                                      "IDNo",
                                                      "lblIDNoTaxIDNo",
                                                      "IDNo");

                        //if (CommonUtil.IsNullOrEmpty(data.BranchNo))
                        //    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                        //                              "MAS020",
                        //                              MessageUtil.MODULE_COMMON,
                        //                              MessageUtil.MessageList.MSG0007,
                        //                              "BranchNo",
                        //                              "lblBranchNo",
                        //                              "BranchNo");

                        //if (CommonUtil.IsNullOrEmpty(data.BranchNameEN))
                        //    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                        //                              "MAS020",
                        //                              MessageUtil.MODULE_COMMON,
                        //                              MessageUtil.MessageList.MSG0007,
                        //                              "BranchNameEN",
                        //                              "lblBranchNameEN",
                        //                              "BranchNameEN");

                        //if (CommonUtil.IsNullOrEmpty(data.BranchNameLC))
                        //    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                        //                              "MAS020",
                        //                              MessageUtil.MODULE_COMMON,
                        //                              MessageUtil.MessageList.MSG0007,
                        //                              "BranchNameLC",
                        //                              "lblBranchNameLC",
                        //                              "BranchNameLC");

                        ValidatorUtil.BuildErrorMessage(res, validator);
                        if (res.IsError)
                            return Json(res);
                    }
                    //End Add

                    if (data.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC)
                    {
                        if (CommonUtil.IsNullOrEmpty(data.CompanyTypeCode))
                        {
                            string lblCompanyType = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblCompanyType");
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                    "MAS020",
                                    MessageUtil.MODULE_COMMON,
                                    MessageUtil.MessageList.MSG0007,
                                    new string[] { lblCompanyType },
                                    new string[] { "CompanyTypeCode" });
                            return Json(res);
                        }
                    }

                    //Add by Jutarat A. on 25122013
                    if (data.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                        || data.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                        || data.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                    {
                        if (CommonUtil.IsNullOrEmpty(data.IDNo) == false && data.IDNo.Length != 15)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                            "MAS020",
                                            MessageUtil.MODULE_MASTER,
                                            MessageUtil.MessageList.MSG1060,
                                            null,
                                            new string[] { "IDNo" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }

                        if (CommonUtil.IsNullOrEmpty(data.BranchNo) == false && data.BranchNo.Length != 5)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                            "MAS020",
                                            MessageUtil.MODULE_MASTER,
                                            MessageUtil.MessageList.MSG1061,
                                            null,
                                            new string[] { "BranchNo" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }

                        if (data.BranchType == "2" && data.BranchNo == "00000")
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                            "MAS020",
                                            MessageUtil.MODULE_MASTER,
                                            MessageUtil.MessageList.MSG1062,
                                            null,
                                            new string[] { "BranchNo" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                    //End Add                    
                }

                MAS020_ScreenParameter param = GetScreenObject<MAS020_ScreenParameter>();

                tbm_BillingClient curBillingClient = param.currentBillingClient;

                IBillingClientMasterHandler hand = ServiceContainer.GetService<IBillingClientMasterHandler>() as IBillingClientMasterHandler;
                List<tbm_BillingClient> oldBillingClient = hand.GetTbm_BillingClient(curBillingClient.BillingClientCode);

                if (!(oldBillingClient.Count > 0) || DateTime.Compare(oldBillingClient[0].UpdateDate.Value, curBillingClient.UpdateDate.Value) != 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    return Json(res);
                }

                
                //string BillingClientCode = curBillingClient.BillingClientCode ;
                curBillingClient= data;

                curBillingClient.BillingClientCode = param.currentBillingClient.BillingClientCode;
                curBillingClient.CreateBy = param.currentBillingClient.CreateBy;
                curBillingClient.CreateDate = param.currentBillingClient.CreateDate;
                curBillingClient.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                curBillingClient.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                List<tbm_BillingClient> list = hand.UpdateBillingClient(curBillingClient);

                string result = (list.Count > 0) ? "1" : null;

                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get customer full name and change english language
        /// </summary>
        /// <param name="NameEN"></param>
        /// <param name="CompanyTypeCode"></param>
        /// <returns></returns>
        public ActionResult MAS020_CustomerNameENChange(string NameEN ,string CompanyTypeCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS020_ScreenParameter param = GetScreenObject<MAS020_ScreenParameter>();          
                res.ResultData = MAS020_GetCustomerFullName(param.CompanyTypeList, NameEN, CompanyTypeCode, true);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get customer full name and change local language
        /// </summary>
        /// <param name="NameLC"></param>
        /// <param name="CompanyTypeCode"></param>
        /// <returns></returns>
        public ActionResult MAS020_CustomerNameLCChange(string NameLC, string CompanyTypeCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS020_ScreenParameter param = GetScreenObject<MAS020_ScreenParameter>();
                res.ResultData = MAS020_GetCustomerFullName(param.CompanyTypeList, NameLC, CompanyTypeCode, false);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get customer full name
        /// </summary>
        /// <param name="CompanyTypeList"></param>
        /// <param name="CustomerName"></param>
        /// <param name="CompanyTypeCode"></param>
        /// <param name="isEN"></param>
        /// <returns></returns>
        private string MAS020_GetCustomerFullName(List<doCompanyType> CompanyTypeList, string CustomerName, string CompanyTypeCode , bool isEN)
        {
            string result = string.Empty;
            string prefix = string.Empty;
            string suffix = string.Empty;
            if (CommonUtil.IsNullOrEmpty(CustomerName))
            {
                return string.Empty;
            }

            List<doCompanyType> list = (from p in CompanyTypeList where p.CompanyTypeCode == CompanyTypeCode select p).ToList<doCompanyType>();

            if (list.Count > 0)
            {
                if (isEN)
                {
                    prefix = list[0].CustNamePrefixEN;
                    suffix = list[0].CustNameSuffixEN;
                }
                else
                {
                    prefix = list[0].CustNamePrefixLC;
                    suffix = list[0].CustNameSuffixLC;
                }

                prefix = (prefix == null) ? "" : prefix + " ";
                suffix = (suffix == null) ? "" : " " + suffix;
                result = prefix + CustomerName + suffix;
            }
            else 
            {
                result = CustomerName;
            }
            

            return result;
        }
      
    }
}

