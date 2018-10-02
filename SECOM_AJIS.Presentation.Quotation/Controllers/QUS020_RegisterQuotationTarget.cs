using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Quotation.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Transactions;

namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    public partial class QuotationController : BaseController
    {
        private const string QUS020_SCREEN_NAME = "QUS020";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult QUS020_Authority(QUS020_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Check screen permission

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_QTN_TARGET, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<QUS020_ScreenParameter>(QUS020_SCREEN_NAME, param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(QUS020_SCREEN_NAME)]
        public ActionResult QUS020()
        {
            return View();
        }

        #endregion
        #region Actions

        //public ActionResult QUS020_GetEmployeeName(string empNo)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
        //        if (empNo != null)
        //        {
        //            IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

        //            List<tbm_Employee> empLst = new List<tbm_Employee>();
        //            empLst.Add(new tbm_Employee() { EmpNo = empNo });

        //            List<tbm_Employee> lst = handler.GetEmployeeList(empLst);
        //            if (lst.Count > 0)
        //                res.ResultData = lst[0].EmpFullName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}
        /// <summary>
        /// Get quotation data from session by selected object type
        /// </summary>
        /// <param name="ObjectTypeID"></param>
        /// <returns></returns>
        public ActionResult QUS020_GetInitialQuotationData(int ObjectTypeID)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                QUS020_InitialQuotationTargetData.INITIAL_OBJECT objID = (QUS020_InitialQuotationTargetData.INITIAL_OBJECT)Enum.ToObject(typeof(QUS020_InitialQuotationTargetData.INITIAL_OBJECT), ObjectTypeID);
                res.ResultData = QUS020_GetInitialData(objID);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Set quotation data from screen to data in session
        /// </summary>
        /// <param name="initData"></param>
        /// <returns></returns>
        public ActionResult QUS020_SetInitialQuotationData(QUS020_InitialQuotationTargetData initData)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res.ResultData = QUS020_SetInitialData(initData);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS020_RetrieveCustomer(QUS020_RetrieveCustomerCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Validate Data

                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }


                #endregion
                #region Get Data

                doCustomer custDo = null;
                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                string CustCodeLong = cmm.ConvertCustCode(cond.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                List<doCustomer> lst = handler.GetCustomerByLanguage(CustCodeLong);
                if (lst.Count > 0)
                    custDo = lst[0];

                if (custDo != null)
                {
                    QUS020_InitialQuotationTargetData initialData = new QUS020_InitialQuotationTargetData();
                    initialData.ObjectTypeID = cond.CustType;

                    if (cond.CustType == 1)
                        initialData.doContractTargetData = custDo;
                    else
                        initialData.doRealCustomerData = custDo;

                    QUS020_SetInitialData(initialData);
                    res.ResultData = custDo;
                }
                else
                {
                    MessageUtil.MessageList msgCode = MessageUtil.MessageList.MSG0068;
                    if (cond.CustType == 2)
                        msgCode = MessageUtil.MessageList.MSG0078;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, msgCode, null, new string[] { "CustCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Copy customer from purchaser customer to real customer
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS020_CopyCustomer()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                QUS020_InitialQuotationTargetData initialData =
                    QUS020_GetInitialData(QUS020_InitialQuotationTargetData.INITIAL_OBJECT.INITIAL_DATA) as QUS020_InitialQuotationTargetData;
                if (initialData == null)
                    initialData = new QUS020_InitialQuotationTargetData();
                initialData.ObjectType = QUS020_InitialQuotationTargetData.INITIAL_OBJECT.INITIAL_DATA;

                if (initialData.doContractTargetData != null)
                {
                    initialData.doRealCustomerData = CommonUtil.CloneObject<doCustomer, doCustomer>(initialData.doContractTargetData);

                    List<object> objRes = new List<object>();
                    objRes.Add(initialData.doRealCustomerData);
                    objRes.Add(true);

                    if (initialData.doQuotationSiteData != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(initialData.doQuotationSiteData.SiteCode) == false
                            && initialData.doQuotationSiteData.CustCode != initialData.doRealCustomerData.CustCode)
                        {
                            initialData.doQuotationSiteData = null;
                            objRes[1] = false;
                        }
                    }

                    QUS020_SetInitialData(initialData);
                    res.ResultData = objRes;
                }
                else
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0073);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS020_RetrieveSiteData(QUS020_RetrieveSiteCondition cond)
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

                #region Get Site Data

                doSite siteDo = null;

                CommonUtil cmm = new CommonUtil();
                ISiteMasterHandler handler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;

                string fullSiteCustCode = CommonUtil.TextCodeName(
                                                cmm.ConvertSiteCode(cond.SiteCustCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                                cond.SiteNo,
                                                "-");
                string longCustCode = cmm.ConvertCustCode(cond.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                List<doSite> lst = handler.GetSite(fullSiteCustCode, longCustCode);
                if (lst.Count > 0)
                    siteDo = lst[0];

                if (siteDo != null)
                {
                    QUS020_InitialQuotationTargetData initialData = new QUS020_InitialQuotationTargetData();
                    initialData.ObjectType = QUS020_InitialQuotationTargetData.INITIAL_OBJECT.QUOTATION_SITE_DATA;
                    initialData.doQuotationSiteData = siteDo;
                    QUS020_SetInitialData(initialData);

                    res.ResultData = siteDo;
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0071, null, new string[] { "SiteCustCodeNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check real customer code has value when clicking seach site
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS020_CheckRealCustomer(QUS020_RetrieveCustomerCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                ValidatorUtil.BuildErrorMessage(res, this);
                if (res.IsError)
                    return Json(res);
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Copy site data from purchaser customer or real customer to site data
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="registerData"></param>
        /// <returns></returns>
        public ActionResult QUS020_CopySiteInfomation(QUS020_RetrieveCustomerCondition cond, QUS020_InitialQuotationTargetData registerData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                QUS020_InitialQuotationTargetData.INITIAL_OBJECT objID = (QUS020_InitialQuotationTargetData.INITIAL_OBJECT)Enum.ToObject(typeof(QUS020_InitialQuotationTargetData.INITIAL_OBJECT), cond.CustType);


                doSite siteDo = null;
                doCustomer doCustomer = QUS020_GetInitialData(objID) as doCustomer;
                if (cond.CustType.ToString() == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                {
                    if (doCustomer == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0073);
                        return Json(res);
                    }

                    siteDo = new doSite()
                    {
                        SiteNameEN = CommonUtil.TextList(new string[] { doCustomer.CustFullNameEN, registerData.BranchNameEN }, " "),
                        SiteNameLC = CommonUtil.TextList(new string[] { doCustomer.CustFullNameLC, registerData.BranchNameLC }, " "),
                        AddressEN = doCustomer.AddressEN,
                        AddressLC = doCustomer.AddressLC,
                        AlleyEN = doCustomer.AlleyEN,
                        AlleyLC = doCustomer.AlleyLC,
                        RoadEN = doCustomer.RoadEN,
                        RoadLC = doCustomer.RoadLC,
                        SubDistrictEN = doCustomer.SubDistrictEN,
                        SubDistrictLC = doCustomer.SubDistrictLC,
                        AddressFullEN = doCustomer.AddressFullEN,
                        AddressFullLC = doCustomer.AddressFullLC,
                        DistrictCode = doCustomer.DistrictCode,
                        ProvinceCode = doCustomer.ProvinceCode,
                        ZipCode = doCustomer.ZipCode,
                        PhoneNo = doCustomer.PhoneNo,
                        ValidateSiteData = false
                    };

                }
                else if (cond.CustType.ToString() == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                {
                    if (doCustomer == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0074);
                        return Json(res);
                    }

                    siteDo = new doSite()
                    {
                        SiteNameEN = doCustomer.CustFullNameEN,
                        SiteNameLC = doCustomer.CustFullNameLC,
                        AddressEN = doCustomer.AddressEN,
                        AddressLC = doCustomer.AddressLC,
                        AlleyEN = doCustomer.AlleyEN,
                        AlleyLC = doCustomer.AlleyLC,
                        RoadEN = doCustomer.RoadEN,
                        RoadLC = doCustomer.RoadLC,
                        SubDistrictEN = doCustomer.SubDistrictEN,
                        SubDistrictLC = doCustomer.SubDistrictLC,
                        AddressFullEN = doCustomer.AddressFullEN,
                        AddressFullLC = doCustomer.AddressFullLC,
                        DistrictCode = doCustomer.DistrictCode,
                        ProvinceCode = doCustomer.ProvinceCode,
                        ZipCode = doCustomer.ZipCode,
                        PhoneNo = doCustomer.PhoneNo,
                        ValidateSiteData = false
                    };
                }

                QUS020_InitialQuotationTargetData initialData = new QUS020_InitialQuotationTargetData();
                initialData.ObjectType = QUS020_InitialQuotationTargetData.INITIAL_OBJECT.QUOTATION_SITE_DATA;
                initialData.doQuotationSiteData = siteDo;
                QUS020_SetInitialData(initialData);

                res.ResultData = siteDo;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when click register quotation data
        /// </summary>
        /// <param name="registerData"></param>
        /// <returns></returns>
        public ActionResult QUS020_RegisterQuotationData(QUS020_InitialQuotationTargetData registerData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CommonUtil cmm = new CommonUtil();

                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
                #region Get Initial Data

                QUS020_InitialQuotationTargetData initData =
                    QUS020_GetInitialData(QUS020_InitialQuotationTargetData.INITIAL_OBJECT.INITIAL_DATA) as QUS020_InitialQuotationTargetData;
                if (initData == null)
                    initData = new QUS020_InitialQuotationTargetData();

                #endregion
                #region Create Do / Check Quotation Target

                doRegisterQuotationTargetData register = QUS020_ConvertToRegisterQuotationData(initData, registerData);
                if (QUS020_CheckQuotationTargetData(res, register) == false)
                    return Json(res);
                QUS020_RegisterData = register;

                #endregion

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when click confirm data
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS020_ConfirmData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CommonUtil cmm = new CommonUtil();

                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Register quotation target data

                doRegisterQuotationTargetData register = QUS020_RegisterData;
                string quotationTargetCode = QUS020_RegisterQuotationTargetData(res, register);
                if (res.IsError)
                    return Json(res);

                if (quotationTargetCode != null)
                    quotationTargetCode = cmm.ConvertQuotationTargetCode(quotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                res.ResultData = quotationTargetCode;

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Initial data from import file
        /// </summary>
        /// <param name="ImportKey"></param>
        /// <returns></returns>
        public ActionResult QUS020_InitImportData(string ImportKey)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                /* --- Import result --- */
                /* --------------------- */
                doInitImportData initData = new doInitImportData();
                /* --------------------- */

                dsImportData importDo = QUS050_GetImportData(ImportKey);
                if (importDo == null)
                    return Json(res);

                #region Set Contract Target / Real Customer Data

                bool hasRealCustomerCode = false;
                bool hasRealCustomerData = false;
                for (int idx = 0; idx < importDo.dtTbt_QuotationCustomer.Count; idx++)
                {
                    tbt_QuotationCustomer cust = importDo.dtTbt_QuotationCustomer[idx];
                    if (CommonUtil.IsNullOrEmpty(cust.CustCode) == false)
                    {
                        List<doCustomer> lst = handler.GetCustomerByLanguage(cmm.ConvertCustCode(cust.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                        if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                        {
                            if (lst.Count > 0)
                                initData.doContractTargetData = lst[0];
                            else
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0068, null, new string[] { "CPSearchCustCode" });
                                initData.doContractTargetData = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(cust);
                                initData.doContractTargetData.CustCode = cmm.ConvertCustCode(cust.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                            }

                        }
                        else
                        {
                            hasRealCustomerCode = true;
                            if (lst.Count > 0)
                            {
                                hasRealCustomerData = true;
                                initData.doRealCustomerData = lst[0];
                            }
                            else
                            {
                                hasRealCustomerData = false;
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0078, null, new string[] { "RCSearchCustCode" });
                                initData.doRealCustomerData = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(cust);
                                initData.doRealCustomerData.CustCode = cmm.ConvertCustCode(cust.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                            }
                        }
                    }
                    else
                    {
                        if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                        {
                            initData.doContractTargetData = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(cust);
                            handler.ValidateCustomerData(initData.doContractTargetData);
                            if (initData.doContractTargetData != null)
                            {
                                if (initData.doContractTargetData.ValidateCustomerData == false)
                                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0126);
                            }
                        }
                        else
                        {
                            initData.doRealCustomerData = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(cust);
                            handler.ValidateCustomerData(initData.doRealCustomerData);
                            if (initData.doRealCustomerData != null)
                            {
                                if (initData.doRealCustomerData.ValidateCustomerData == false)
                                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0127);
                                else
                                    hasRealCustomerData = true;
                            }
                        }
                    }
                }

                #endregion
                #region Set Site Data

                if (importDo.dtTbt_QuotationSite != null)
                {
                    if (importDo.dtTbt_QuotationSite.Count > 0)
                    {
                        initData.doQuotationSiteData = CommonUtil.CloneObject<tbt_QuotationSite, doSite>(importDo.dtTbt_QuotationSite[0]);

                        ISiteMasterHandler smhandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;

                        bool isSetSite = false;
                        if (initData.doQuotationSiteData != null)
                            isSetSite = !CommonUtil.IsNullOrEmpty(initData.doQuotationSiteData.SiteNo);

                        if (hasRealCustomerCode && isSetSite)
                        {
                            string fullSiteCustCode = CommonUtil.TextCodeName(initData.doRealCustomerData.SiteCustCode,
                                                                                initData.doQuotationSiteData.SiteNo,
                                                                                "-");

                            List<doSite> smlst = smhandler.GetSite(fullSiteCustCode, initData.doRealCustomerData.CustCode);
                            if (smlst.Count > 0)
                                initData.doQuotationSiteData = smlst[0];
                            else if (hasRealCustomerData)
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0071, null, new string[] { "SiteCustCodeNo" });
                            else
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0072, null, new string[] { "SiteCustCodeNo" });
                        }
                        else if (isSetSite)
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2030, null, new string[] { "SiteCustCodeNo" });
                        else
                        {
                            /* --- Validate and set Site data --- */
                            bool isSiteError = false;
                            smhandler.ValidateSiteData(initData.doQuotationSiteData);
                            if (initData.doQuotationSiteData != null)
                            {
                                if (initData.doQuotationSiteData.ValidateSiteData == false)
                                    isSiteError = true;
                            }
                            if (isSiteError)
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0128);
                        }
                    }
                }

                #endregion
                #region More Information Data

                if (importDo.dtTbt_QuotationTarget.Count > 0)
                {

                    List<string> param = new List<string>();
                    List<string> controls = new List<string>();
                    initData.doQuotationTargetData = CommonUtil.CloneObject<tbt_QuotationTarget, tbt_QuotationTarget>(importDo.dtTbt_QuotationTarget[0]);

                    #region Validate Combo Data

                    QUS020_tbt_QuotationTarget_InitCombo combo =
                        CommonUtil.CloneObject<tbt_QuotationTarget, QUS020_tbt_QuotationTarget_InitCombo>(initData.doQuotationTargetData);

                    if (CommonUtil.IsNullOrEmpty(combo.ProductTypeCode) == false)
                    {
                        ICommonHandler cmmHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<tbs_ProductType> lst = cmmHandler.GetTbs_ProductType(null, combo.ProductTypeCode);
                        if (lst.Count > 0)
                            combo.ProductTypeName = lst[0].ProductTypeName;
                    }

                    MiscTypeMappingList miscLst = new MiscTypeMappingList();
                    miscLst.AddMiscType(combo);

                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    chandler.MiscTypeMappingList(miscLst);

                    ValidatorUtil.BuildErrorMessage(res, new object[] { combo }, null, false);

                    #endregion

                    //if (initData.doQuotationTargetData.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE
                    //    && CommonUtil.IsNullOrEmpty(initData.doQuotationTargetData.OldContractCode) == false)
                    //{
                    //    initData.doQuotationTargetData.OldContractCode = null;
                    //    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2032);
                    //}
                    //if ((initData.doQuotationTargetData.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_CUST
                    //    || initData.doQuotationTargetData.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY
                    //    || initData.doQuotationTargetData.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_SUBCONTRACTOR) == false
                    //    && CommonUtil.IsNullOrEmpty(initData.doQuotationTargetData.IntroducerCode) == false)
                    //{
                    //    initData.doQuotationTargetData.IntroducerCode = null;
                    //    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2033);
                    //}
                }

                #endregion

                QUS020_InitialQuotationTargetData initialData = new QUS020_InitialQuotationTargetData();
                initialData.ObjectType = QUS020_InitialQuotationTargetData.INITIAL_OBJECT.INITIAL_DATA;
                
                initialData.doContractTargetData = CommonUtil.CloneObject<doCustomer, doCustomer>(initData.doContractTargetData);
                initialData.doRealCustomerData = CommonUtil.CloneObject<doCustomer, doCustomer>(initData.doRealCustomerData);
                initialData.doQuotationSiteData = CommonUtil.CloneObject<doSite, doSite>(initData.doQuotationSiteData);

                QUS020_SetInitialData(initialData);

                #region Update Import key

                QUS020_ScreenParameter p = GetScreenObject<QUS020_ScreenParameter>();
                p.ImportKey = ImportKey;
                UpdateScreenObject(p);

                #endregion

                res.ResultData = initData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Initial enviroment before re-direct to QUS030 screen
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS020_RegisterQuotationDetail()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                string ImportKey = null;
                
                QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                if (param != null)
                {
                    ImportKey = param.ImportKey;
                    param.LoadImportSessionToDetail = true;
                }

                res.ResultData = ImportKey;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Remove data from session
        /// </summary>
        public void QUS020_ClearSession()
        {
            try
            {
                QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                if (param != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.ImportKey) == false)
                    {
                        dsImportData importDo = QUS050_GetImportData(param.ImportKey);
                        if (importDo != null)
                        {
                            if (param.LoadImportSessionToDetail == false
                                || CommonUtil.IsNullOrEmpty(importDo.dtTbt_QuotationBasic[0].ProductCode))
                            {
                                //Clear Import session.
                                UpdateScreenObject(null, param.ImportKey);
                            }
                        }
                    }
                }
                
                //Clear current session.
                UpdateScreenObject(null);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Reset data in session to begining state
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS020_ResetSessionData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                if (param != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.ImportKey) == false)
                    {
                        dsImportData importDo = QUS050_GetImportData(param.ImportKey);
                        if (importDo != null)
                        {
                            //Clear Import session.
                            UpdateScreenObject(null, param.ImportKey);
                        }
                    }

                    ScreenParameter nparam = ScreenParameter.ResetScreenParameter(param);
                    if (nparam != null)
                        UpdateScreenObject(nparam);
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Get data from session by object id
        /// </summary>
        /// <param name="ObjectID"></param>
        /// <returns></returns>
        private object QUS020_GetInitialData(QUS020_InitialQuotationTargetData.INITIAL_OBJECT ObjectID)
        {
            try
            {
                QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                if (param != null)
                {
                    if (param.InitialData != null)
                        return param.InitialData.GetObjectData(ObjectID);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Set data to session
        /// </summary>
        /// <param name="initialData"></param>
        /// <returns></returns>
        private bool QUS020_SetInitialData(QUS020_InitialQuotationTargetData initialData)
        {
            try
            {
                QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                if (param.InitialData == null)
                    param.InitialData = new QUS020_InitialQuotationTargetData();
                param.InitialData.SetObjectData(initialData);

                //UpdateScreenObject(param);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get/Set register data in session
        /// </summary>
        private doRegisterQuotationTargetData QUS020_RegisterData
        {
            get
            {
                //QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                //if (param != null)
                //{
                //    if (param.RegisterData != null)
                //        return param.RegisterData;
                //}
                //return null;

                QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                if (param.RegisterData != null)
                    return param.RegisterData;
                return null;
            }
            set
            {
                //QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                //if (param == null)
                //    param = new QUS020_ScreenParameter();
                //param.RegisterData = value;

                //UpdateScreenObject(param);

                QUS020_ScreenParameter param = GetScreenObject<QUS020_ScreenParameter>();
                param.RegisterData = value;

                UpdateScreenObject(param);
            }
        }
        /// <summary>
        /// Convert initial data and input data from screen to register data object
        /// </summary>
        /// <param name="initData"></param>
        /// <param name="registerData"></param>
        /// <returns></returns>
        private doRegisterQuotationTargetData QUS020_ConvertToRegisterQuotationData(QUS020_InitialQuotationTargetData initData, QUS020_InitialQuotationTargetData registerData)
        {
            try
            {
                CommonUtil cmm = new CommonUtil();

                #region Get Prefix Code/Service Type Code

                string prefixCode = null;
                string serviceTypeCode = null;
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<tbs_ProductType> lstProductType = hand.GetTbs_ProductType(null, registerData.ProductTypeCode);
                if (lstProductType.Count > 0)
                {
                    prefixCode = lstProductType[0].QuotationPrefix;
                    serviceTypeCode = lstProductType[0].ServiceTypeCode;
                }

                #endregion
                #region Create Register Target Data

                doRegisterQuotationTargetData register = new doRegisterQuotationTargetData();
                register.BranchContractFlag = registerData.IsBranchChecked;

                register.doTbt_QuotationTarget = new tbt_QuotationTarget()
                {
                    QuotationTargetCode = null,
                    BranchNameEN = registerData.BranchNameEN,
                    BranchNameLC = registerData.BranchNameLC,
                    BranchAddressEN = registerData.BranchAddressEN,
                    BranchAddressLC = registerData.BranchAddressLC,
                    ProductTypeCode = registerData.ProductTypeCode,
                    PrefixCode = prefixCode,
                    ServiceTypeCode = serviceTypeCode,
                    TargetCodeTypeCode = SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE,
                    QuotationOfficeCode = registerData.QuotationOfficeCode,
                    OperationOfficeCode = registerData.OperationOfficeCode,
                    AcquisitionTypeCode = registerData.AcquisitionTypeCode,
                    IntroducerCode = registerData.IntroducerCode,
                    MotivationTypeCode = registerData.MotivationTypeCode,
                    OldContractCode = cmm.ConvertContractCode(registerData.OldContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                    QuotationStaffEmpNo = registerData.QuotationStaffEmpNo,
                    LastAlphabet = null,
                    ContractTransferStatus = SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG,
                    ContractCode = null,
                    TransferDate = null,
                    TransferAlphabet = null,

                    ContractTargetMemo = registerData.ContractTargetMemo,
                    RealCustomerMemo = registerData.RealCustomerMemo
                };

                if (initData.doContractTargetData != null)
                {
                    if (initData.doContractTargetData.CustCode != null)
                    {
                        register.doTbt_QuotationCustomer1 = new tbt_QuotationCustomer();
                        register.doTbt_QuotationCustomer1.QuotationTargetCode = null;
                        register.doTbt_QuotationCustomer1.CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET;
                        register.doTbt_QuotationCustomer1.CustCode = initData.doContractTargetData.CustCode;
                    }
                    else
                    {
                        register.doTbt_QuotationCustomer1 = CommonUtil.CloneObject<doCustomer, tbt_QuotationCustomer>(initData.doContractTargetData);
                        if (register.doTbt_QuotationCustomer1 == null)
                            register.doTbt_QuotationCustomer1 = new tbt_QuotationCustomer();

                        register.doTbt_QuotationCustomer1.QuotationTargetCode = null;
                        register.doTbt_QuotationCustomer1.CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET;
                        register.doTbt_QuotationCustomer1.CustCode = null;
                    }
                }

                if (initData.doRealCustomerData != null)
                {
                    if (initData.doRealCustomerData.CustCode != null)
                    {
                        register.doTbt_QuotationCustomer2 = new tbt_QuotationCustomer();
                        register.doTbt_QuotationCustomer2.QuotationTargetCode = null;
                        register.doTbt_QuotationCustomer2.CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST;
                        register.doTbt_QuotationCustomer2.CustCode = initData.doRealCustomerData.CustCode;
                    }
                    else
                    {
                        register.doTbt_QuotationCustomer2 = CommonUtil.CloneObject<doCustomer, tbt_QuotationCustomer>(initData.doRealCustomerData);
                        if (register.doTbt_QuotationCustomer2 == null)
                            register.doTbt_QuotationCustomer2 = new tbt_QuotationCustomer();

                        register.doTbt_QuotationCustomer2.QuotationTargetCode = null;
                        register.doTbt_QuotationCustomer2.CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST;
                        register.doTbt_QuotationCustomer2.CustCode = null;
                    }
                }

                if (initData.doQuotationSiteData != null)
                {
                    if (initData.doQuotationSiteData.SiteCode != null)
                    {
                        register.doTbt_QuotationSite = new tbt_QuotationSite();
                        register.doTbt_QuotationSite.QuotationTargetCode = null;
                        register.doTbt_QuotationSite.SiteCode = initData.doQuotationSiteData.SiteCode;
                        register.doTbt_QuotationSite.SiteNo = initData.doQuotationSiteData.SiteCode.Substring(initData.doQuotationSiteData.SiteCode.Length - 4);
                    }
                    else
                    {
                        register.doTbt_QuotationSite = CommonUtil.CloneObject<doSite, tbt_QuotationSite>(initData.doQuotationSiteData);
                        if (register.doTbt_QuotationSite == null)
                            register.doTbt_QuotationSite = new tbt_QuotationSite();

                        register.doTbt_QuotationSite.QuotationTargetCode = null;
                        register.doTbt_QuotationSite.SiteCode = null;
                        register.doTbt_QuotationSite.SiteNo = null;
                    }
                }

                #endregion

                return register;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check quotation target data is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        private bool QUS020_CheckQuotationTargetData(ObjectResultData res, doRegisterQuotationTargetData register)
        {
            try
            {
                #region Mandatory Check

                QUS020_doRegisterQuotationTargetData reg =
                    CommonUtil.CloneObject<doRegisterQuotationTargetData, QUS020_doRegisterQuotationTargetData>(register);
                QUS020_tbt_QuotationTarget qt =
                    CommonUtil.CloneObject<tbt_QuotationTarget, QUS020_tbt_QuotationTarget>(register.doTbt_QuotationTarget);
                qt.BranchContractFlag = register.BranchContractFlag;

                ValidatorUtil.BuildErrorMessage(res, new object[] { reg, qt });
                if (res.IsError)
                    return false;

                #endregion
                #region Business Check

                ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;

                bool hasContractCustCode = false;
                bool hasRealCustCode = false;
                bool hasSiteCode = false;
                
                if (register.doTbt_QuotationCustomer1 != null)
                {
                    if (CommonUtil.IsNullOrEmpty(register.doTbt_QuotationCustomer1.CustCode) == false)
                        hasContractCustCode = true;
                }
                if (register.doTbt_QuotationCustomer2 != null)
                {
                    if (CommonUtil.IsNullOrEmpty(register.doTbt_QuotationCustomer2.CustCode) == false)
                        hasRealCustCode = true;

                    if (register.doTbt_QuotationSite != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(register.doTbt_QuotationCustomer2.CustCode) == false
                            && CommonUtil.IsNullOrEmpty(register.doTbt_QuotationSite.SiteCode) == false)
                        {
                            hasSiteCode = true;
                        }
                    }
                }

                #region Check contract target

                if (hasContractCustCode == false)
                {
                    doCustomer cust = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(register.doTbt_QuotationCustomer1);
                    chandler.ValidateCustomerData(cust);
                    if (cust != null)
                    {
                        if (cust.ValidateCustomerData == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0126);
                            return false;
                        }
                    }
                }
                else
                {
                    if (chandler.CheckActiveCustomerData(register.doTbt_QuotationCustomer1.CustCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0068, null, new string[] { "CPSearchCustCode" });
                        return false;
                    }
                }

                #endregion
                #region Real customer

                if (hasRealCustCode == false)
                {
                    if (register.doTbt_QuotationCustomer2 != null)
                    {
                        doCustomer cust = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(register.doTbt_QuotationCustomer2);
                        chandler.ValidateCustomerData(cust);
                        if (cust != null)
                        {
                            if (cust.ValidateCustomerData == false)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0127);
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (chandler.CheckActiveCustomerData(register.doTbt_QuotationCustomer2.CustCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0078, null, new string[] { "RCSearchCustCode" });
                        return false;
                    }
                }

                #endregion
                #region Real customer memo

                if (register.doTbt_QuotationCustomer2 == null
                    && CommonUtil.IsNullOrEmpty(register.doTbt_QuotationTarget.RealCustomerMemo) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2051);
                    return false;
                }

                #endregion
                #region Site

                if (hasSiteCode == false)
                {
                    doSite s = CommonUtil.CloneObject<tbt_QuotationSite, doSite>(register.doTbt_QuotationSite);
                    shandler.ValidateSiteData(s);
                    if (s != null)
                    {
                        if (s.ValidateSiteData == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0128);
                            return false;
                        }
                    }
                }
                else
                {
                    if (hasRealCustCode == false)
                    {
                        if (CommonUtil.IsNullOrEmpty(register.doTbt_QuotationSite.SiteCode) == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2035, null, new string[] { "SiteCustCodeNo" });
                            return false;
                        }
                    }
                    else if (shandler.CheckExistSiteData(register.doTbt_QuotationSite.SiteCode,
                                                            register.doTbt_QuotationCustomer2.CustCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2036, null, new string[] { "SiteCustCodeNo" });
                        return false;
                    }
                }

                #endregion
                #region Product type

                if (register.doTbt_QuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE
                    && register.doTbt_QuotationTarget.OldContractCode != null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2034, null, new string[] { "OldContractCode" });
                    return false;
                }

                #endregion
                #region Acquistion type code

                if ((register.doTbt_QuotationTarget.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_CUST
                    || register.doTbt_QuotationTarget.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY) == false
                    && CommonUtil.IsNullOrEmpty(register.doTbt_QuotationTarget.IntroducerCode) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2033, null, new string[] { "IntroducerCode" });
                    return false;
                }

                #endregion
                #region Product type of Old contract code

                if (register.doTbt_QuotationTarget.OldContractCode != null)
                {
                    bool isError = true;
                    IContractHandler handler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

                    #region Check contract data

                    List<tbt_SaleBasic> lst = new List<tbt_SaleBasic>()
                        {
                            new tbt_SaleBasic()
                            {
                                ContractCode = register.doTbt_QuotationTarget.OldContractCode
                            }
                        };
                    List<doContractHeader> clst = handler.GetContractHeaderData(lst);
                    if (clst != null)
                    {
                        if (clst.Count > 0)
                        {
                            if (clst[0].ContractCode2 != null)
                                isError = false;
                        }
                    }
                    if (isError)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2038, null, new string[] { "OldContractCode" });
                        return false;
                    }

                    #endregion
                    #region Check product type AL/Online

                    if (register.doTbt_QuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                        || register.doTbt_QuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_ONLINE
                        || register.doTbt_QuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        if (clst[0].ProductTypeCode != SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                            && clst[0].ProductTypeCode != SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_ONLINE
                            && clst[0].ProductTypeCode != SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2039, null, new string[] { "OldContractCode" });
                            return false;
                        }
                    }

                    #endregion
                    #region Check product type BE/SG

                    if (register.doTbt_QuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_BE
                        || register.doTbt_QuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SG)
                    {
                        if (clst[0].ProductTypeCode != SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_BE
                            && clst[0].ProductTypeCode != SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SG)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2040, null, new string[] { "OldContractCode" });
                            return false;
                        }
                    }

                    #endregion
                    #region Check product type MA

                    if (register.doTbt_QuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_MA)
                    {
                        if (clst[0].ProductTypeCode != SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_MA)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2041, null, new string[] { "OldContractCode" });
                            return false;
                        }
                    }

                    #endregion
                }

                #endregion
                #region Quotation staff code

                if (CommonUtil.IsNullOrEmpty(register.doTbt_QuotationTarget.QuotationStaffEmpNo) == false)
                {
                    IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                    if (handler.CheckExistActiveEmployee(register.doTbt_QuotationTarget.QuotationStaffEmpNo) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2037, null, new string[] { "QuotationStaffEmpNo" });
                        return false;
                    }
                }

                #endregion

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Update quotation target data to database
        /// </summary>
        /// <param name="res"></param>
        /// <param name="registerData"></param>
        /// <returns></returns>
        private string QUS020_RegisterQuotationTargetData(ObjectResultData res, doRegisterQuotationTargetData registerData)
        {
            try
            {
                string quotationTargetCode = null;

                /* --- Check Quotation target data --- */
                //if (QUS020_CheckQuotationTargetData(res, registerData) == false)
                //    return null;

                using (TransactionScope scope = new TransactionScope())
                {
                    IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

                    #region Generate Quotation target code

                    quotationTargetCode = handler.GenerateQuotationTargetCode(registerData.doTbt_QuotationTarget.ProductTypeCode);

                    #endregion
                    #region Update Quotation target code

                    registerData.doTbt_QuotationTarget.QuotationTargetCode = quotationTargetCode;
                    registerData.doTbt_QuotationCustomer1.QuotationTargetCode = quotationTargetCode;

                    if (registerData.doTbt_QuotationCustomer2 != null)
                        registerData.doTbt_QuotationCustomer2.QuotationTargetCode = quotationTargetCode;

                    registerData.doTbt_QuotationSite.QuotationTargetCode = quotationTargetCode;

                    #endregion
                    #region Create Quotation target data

                    handler.CreateQuotationTargetData(registerData);

                    #endregion

                    scope.Complete();
                }

                return quotationTargetCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
