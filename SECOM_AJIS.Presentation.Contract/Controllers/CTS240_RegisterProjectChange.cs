
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
//*********************************	
// Create by: Nattapong N.	
// Create date: 20/09/2011	
// Update date: 20/09/2011	
//*********************************	
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Presentation.Contract.Models;
using System.Collections;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region AuthorityAndInitial
        /// <summary>
        /// Check can edit permission
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_EditPermission()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.ResultData = CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_CHANGE, FunctionID.C_FUNC_ID_EDIT);
                return Json(res);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Check can cancel permission
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_CancelPermission()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.ResultData = CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_CHANGE, FunctionID.C_FUNC_ID_CANCEL);
                return Json(res);


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Check last complete permission
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_LastCompletePermission()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.ResultData = CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_CHANGE, FunctionID.C_FUNC_ID_PROJECT_LAST_COMPLETE);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Authority screen CTS240
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_Authority(CTS240_ScreenParameter Cond)
        {
            // Cond.strProjectCode = "p0000132";
            ObjectResultData res = new ObjectResultData();
            try
            {
                //if (!CommonUtil.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ProjectCode))
                //{
                //    Cond.strProjectCode = CommonUtil.dsTransData.dtCommonSearch.ProjectCode;
                //}
                if (Cond.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(Cond.CommonSearch.ProjectCode) == false)
                        Cond.strProjectCode = Cond.CommonSearch.ProjectCode;
                }

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_CHANGE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }
                //if (ModelState.IsValid == false)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                //    ValidatorUtil.BuildErrorMessage(res, this, null);
                //    if (res.IsError)
                //        return Json(res);
                //}
                
                
                
                //if (CommonUtil.IsNullOrEmpty(Cond.strProjectCode))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3216, new string [] { "lblProjectCode" });
                //    return Json(res);
                //}
                //IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                //List<doTbt_Project> doTbt_Project = projH.GetTbt_ProjectForView(Cond.strProjectCode);
                //if (doTbt_Project.Count <= 0)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091);
                //    return Json(res);
                //}
                //else
                //{
                //    Cond.strProjectCode = doTbt_Project[0].ProjectCode;
                //}
                if (CommonUtil.IsNullOrEmpty(Cond.strProjectCode) == false)
                {
                    IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                    List<doTbt_Project> doTbt_Project = projH.GetTbt_ProjectForView(Cond.strProjectCode);
                    if (doTbt_Project.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, new string[] { Cond.strProjectCode });
                        return Json(res);
                    }
                }

                return InitialScreenEnvironment<CTS240_ScreenParameter>("CTS240", Cond, res);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen CTS240
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS240")]
        public ActionResult CTS240()
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
            if (param != null)
                ViewBag.strProjectCode = param.strProjectCode;
            else
                param = new CTS240_ScreenParameter();
            IProjectHandler ProjH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            List<tbt_ProjectStockoutBranchIntrumentDetails> lstBranch;
            lstBranch = ProjH.GetTbt_ProjectStockoutBranchIntrumentDetails(param.strProjectCode);
            if (lstBranch.Count > 0)
                ViewBag.BranchDDL = lstBranch[0].ProjectCodeBranchNo;
            ViewBag.C_PROJECT_STATUS_LASTCOMPLETE = ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE;
            ViewBag.C_PROJECT_STATUS_CANCEL = ProjectStatus.C_PROJECT_STATUS_CANCEL;
            ViewBag.Edit = ActionFlag.Edit;

            ViewBag.AttachKey = GetCurrentKey();

            return View();

        }

        /// <summary>
        /// Initial upload attach section
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_ViewUpload(string strProjectCode)
        {
            List<dtAttachFileNameID> lstFile = new List<dtAttachFileNameID>();
            if (!CommonUtil.IsNullOrEmpty(strProjectCode))
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //lstFile = comh.GetAttachFileName(strProjectCode, null, null);
                ViewBag.AttachFileList = lstFile;
                ViewBag.ProjectCode = strProjectCode;
            }
            return View();
        }

        /// <summary>
        /// Upload attach file
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_Upload(string strProjectCode)
        {
            List<dtAttachFileNameID> lstFile = new List<dtAttachFileNameID>();
            if (!CommonUtil.IsNullOrEmpty(strProjectCode))
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //lstFile = comh.GetAttachFileName(strProjectCode, null, true);
                ViewBag.AttachFileList = lstFile;
                ViewBag.ProjectCode = strProjectCode;
            }
            ViewBag.K = GetCurrentKey();
            return View();
        }

        /// <summary>
        /// Get data tbt_project for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetTbt_ProjectForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.IsNullOrEmpty(strProjectCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS240",
                        MessageUtil.MODULE_COMMON, 
                        MessageUtil.MessageList.MSG0007,
                        new string[] { "lblProjectCode" },
                        new string[] { "ProjectCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<doTbt_Project> doTbt_Project = projH.GetTbt_ProjectForView(strProjectCode);
                if (doTbt_Project.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, 
                        new string[] { strProjectCode }, 
                        new string[] { "ProjectCode" });
                    return Json(res);
                }
                res.ResultData = doTbt_Project[0];
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }
        #endregion
        #region LoadDataGridWithInitial
        /// <summary>
        /// Get data to grid other related for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetOtherRelateForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            List<tbt_ProjectOtherRalatedCompany> lstOther = new List<tbt_ProjectOtherRalatedCompany>();
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

            if (param.doRegProject240 == null)
                param.doRegProject240 = new doRegisterProject240();
            try
            {
                IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                lstOther = Projh.GetTbt_ProjectOtherRalatedCompanyForView(strProjectCode);
                param.doRegProject240.OtherRelate = CommonUtil.ClonsObjectList<tbt_ProjectOtherRalatedCompany, CTS240_OtherRelate>(lstOther);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<tbt_ProjectOtherRalatedCompany>(lstOther, "contract\\CTS240_other", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            UpdateScreenObject(param);
            return Json(res);
        }

        /// <summary>
        /// Get data to grid system for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetSystemDetailForView(string strProjectCode)
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

            if (param.doRegProject240 == null)
                param.doRegProject240 = new doRegisterProject240();
            ObjectResultData res = new ObjectResultData();
            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            List<dtTbt_ProjectSystemDetailForView> lstSysProd = Projh.GetTbt_ProjectSystemDetailForView(strProjectCode);
            param.doRegProject240.SystemProduct = CommonUtil.ClonsObjectList<dtTbt_ProjectSystemDetailForView, CTS240_SystemProduct>(lstSysProd);
            UpdateScreenObject(param);
            res.ResultData = CommonUtil.ConvertToXml<dtTbt_ProjectSystemDetailForView>(lstSysProd, "contract\\CTS240_system", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }


        /// <summary>
        /// Get data to grid expect instrument for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetExpectIntrumentDetailForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.ExpectInstrument == null)
                    param.doRegProject240.ExpectInstrument = new List<CTS240_ExpectInstrument>();

                IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<dtTbt_ProjectExpectedInstrumentDetailsForView> lstIntrument = Projh.GetTbt_ProjectExpectedInstrumentDetailsForView(strProjectCode);
                string result = CommonUtil.ConvertToXml<dtTbt_ProjectExpectedInstrumentDetailsForView>(lstIntrument, "contract\\CTS240_expect", CommonUtil.GRID_EMPTY_TYPE.INSERT);
                param.doRegProject240.ExpectInstrument = CommonUtil.ClonsObjectList<dtTbt_ProjectExpectedInstrumentDetailsForView, CTS240_ExpectInstrument>(lstIntrument);
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
        /// Get data to grid project wip for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetProjectWIPForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

                if (param.lstWip == null)
                    param.lstWip = new List<dtTbt_ProjectStockoutIntrumentForView>();

                IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<dtTbt_ProjectStockoutIntrumentForView> lstWip = Projh.GetTbt_ProjectStockoutIntrumentForView(strProjectCode);
                param.lstWip = lstWip;
                res.ResultData = CommonUtil.ConvertToXml<dtTbt_ProjectStockoutIntrumentForView>(lstWip, "contract\\CTS240_wip", CommonUtil.GRID_EMPTY_TYPE.INSERT);
                UpdateScreenObject(param);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }


        }

        /// <summary>
        /// Get data to grid stockout for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="BranchNo"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetStockOutForView(string strProjectCode, int? BranchNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IProjectHandler projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> lstBranch;
                lstBranch = projh.GetTbt_ProjectStockoutBranchIntrumentDetailForView(strProjectCode, BranchNo);
                List<View_dtTbt_ProjectStockoutBranchIntrumentDetailForView> View_StockOut = new List<View_dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                View_StockOut = CommonUtil.ClonsObjectList<dtTbt_ProjectStockoutBranchIntrumentDetailForView, View_dtTbt_ProjectStockoutBranchIntrumentDetailForView>(lstBranch);
                res.ResultData = CommonUtil.ConvertToXml<View_dtTbt_ProjectStockoutBranchIntrumentDetailForView>(View_StockOut, "contract\\CTS240_stock", CommonUtil.GRID_EMPTY_TYPE.INSERT);

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get data to grid support staff for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetSupportStaffForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

            if (param.doRegProject240 == null)
                param.doRegProject240 = new doRegisterProject240();
            if (param.doRegProject240.SupportStaff == null)
                param.doRegProject240.SupportStaff = new List<CTS240_SupportStaff>();
            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            List<dtTbt_ProjectSupportStaffDetailForView> lstStaff = Projh.GetTbt_ProjectSupportStaffDetailForView(strProjectCode);
            EmployeeMappingList emlst = new EmployeeMappingList();
            emlst.AddEmployee(lstStaff.ToArray());
            IEmployeeMasterHandler Emph = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            Emph.EmployeeListMapping(emlst);
            string result = CommonUtil.ConvertToXml<dtTbt_ProjectSupportStaffDetailForView>(lstStaff, "contract\\CTS240_support", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            res.ResultData = result;

            param.doRegProject240.SupportStaff = CommonUtil.ClonsObjectList<dtTbt_ProjectSupportStaffDetailForView, CTS240_SupportStaff>(lstStaff);
            UpdateScreenObject(param);
            return Json(res);
        }


        public ActionResult CTS240_AttachGrid(string strProjectCode)
        {
            //ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            //List<dtAttachFileNameID> lstFileName = new List<dtAttachFileNameID>();
            //lstFileName = comh.GetAttachFileName(strProjectCode, null, true);
            //string attachGrid;
            //// if(lstFileName.Count>0)
            //attachGrid = CommonUtil.ConvertToXml<dtAttachFileNameID>(lstFileName, "contract\\CTS240_attach", CommonUtil.GRID_EMPTY_TYPE.INSERT);

            //return Json(attachGrid);
            return View();

        }
        #endregion
        #region initEmptyGrid
        /// <summary>
        /// Initial grid other related
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_Other()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS240_other", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }
        /// <summary>
        /// Initial grid support staff
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_support()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS240_support", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }
        /// <summary>
        /// Initial grid system
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_system()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS240_system", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }
        /// <summary>
        /// Initial grid expect instrument
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_expect()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS240_expect", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }
        /// <summary>
        /// Initial grid project wip
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_wip()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS240_wip", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }
        /// <summary>
        /// Initial grid stockout
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_stock()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS240_stock", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }
        /// <summary>
        /// Initial grid attach
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_attach()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS240_attach", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }

        #endregion
        #region Get method
        /// <summary>
        /// Get data project purchaser
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetProjectPurchaser(string strProjectCode)
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            //=============== Teerapong 14/09/2012 ============
            ViewBag.strProjectCode = strProjectCode;
            param.strProjectCode = strProjectCode;
            //=================================================
            try
            {
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.doRegCust == null)
                    param.doRegProject240.doRegCust = new doRegisterCustomer();
                IProjectHandler projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<dtTbt_ProjectPurchaserCustomerForView> lstPurchaser = new List<dtTbt_ProjectPurchaserCustomerForView>();
                lstPurchaser = projh.GetTbt_ProjectPurchaserCustomerForView(strProjectCode);
                if (lstPurchaser.Count > 0)
                {
                    if (!CommonUtil.IsNullOrEmpty(lstPurchaser[0].CustCode))
                    {
                        ICustomerMasterHandler icustMast = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                        List<doCustomer> lstCust = icustMast.GetCustomer(lstPurchaser[0].CustCode);

                        lstPurchaser = CommonUtil.ClonsObjectList<doCustomer, dtTbt_ProjectPurchaserCustomerForView>(lstCust);
                    }
                }
                EmployeeMappingList emlst = new EmployeeMappingList();
                emlst.AddEmployee(lstPurchaser.ToArray());
                IEmployeeMasterHandler Emph = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                Emph.EmployeeListMapping(emlst);
                CommonUtil.MappingObjectLanguage<dtTbt_ProjectPurchaserCustomerForView>(lstPurchaser);

                MiscTypeMappingList miscMapList = new MiscTypeMappingList();
                miscMapList.AddMiscType(lstPurchaser.ToArray());
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comh.MiscTypeMappingList(miscMapList);

                if (lstPurchaser.Count > 0)
                {
                    param.doRegProject240.doRegCust = CommonUtil.CloneObject<dtTbt_ProjectPurchaserCustomerForView, doRegisterCustomer>(lstPurchaser[0]);
                }
                //Add by Jutarat A. on 18012013
                else
                {
                    dtTbt_ProjectPurchaserCustomerForView purchaserTemp = new dtTbt_ProjectPurchaserCustomerForView();
                    lstPurchaser.Add(purchaserTemp);
                }
                //End Add

                UpdateScreenObject(param);
                return Json(lstPurchaser);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get data project for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetProjectForView(string strProjectCode)
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            try
            {
                IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<doTbt_Project> lstProject = Projh.GetTbt_ProjectForView(strProjectCode);
                EmployeeMappingList emlst = new EmployeeMappingList();
                emlst.AddEmployee(lstProject.ToArray());
                IEmployeeMasterHandler Emph = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                Emph.EmployeeListMapping(emlst);

                MiscTypeMappingList miscMapList = new MiscTypeMappingList();
                miscMapList.AddMiscType(lstProject.ToArray());
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comh.MiscTypeMappingList(miscMapList);

                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.StockOut == null)
                    param.doRegProject240.StockOut = new List<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                param.doRegProject240.StockOut = Projh.GetTbt_ProjectStockoutBranchIntrumentDetailForView(param.strProjectCode, null);
                foreach (dtTbt_ProjectStockoutBranchIntrumentDetailForView i in param.doRegProject240.StockOut)
                {
                    if (i.AssignBranchQty == null)
                    {
                        i.AssignBranchQty = 0;
                        i.ActionFlag = ActionFlag.Insert;
                    }
                }
                UpdateScreenObject(param);

                res.ResultData = lstProject;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Get data purchaser 
        /// </summary>
        /// <param name="CustCode">customer code in short format</param>
        /// <returns>doCustomer</returns>
        public ActionResult CTS240_RetrievePurchaserData(string CustCode)
        {
            ObjectResultData res = new ObjectResultData();
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
            if (CommonUtil.IsNullOrEmpty(CustCode))
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0067, null, new string[] { "CPSearchCustCode" });
                return Json(res);
            }
            try
            {
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.doRegCust == null)
                    param.doRegProject240.doRegCust = new doRegisterCustomer();
                ICustomerMasterHandler CustH = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                List<doCustomer> lstCust = CustH.GetCustomerByLanguage(new CommonUtil().ConvertCustCode(CustCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (lstCust.Count > 0)
                    param.doRegProject240.doRegCust = CommonUtil.CloneObject<doCustomer, doRegisterCustomer>(lstCust[0]);
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0068, null, new string[] { "CPSearchCustCode" });
                    return Json(res);
                }
                UpdateScreenObject(param);
                return Json(param.doRegProject240.doRegCust);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }


        }
        #endregion
        #region Validate
        /// <summary>
        /// Validate data before last complete
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_ValidateLastComplete(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            IProjectHandler PrjH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            try
            {
                //1.1.2
                List<doProjectContractDetail> doProjectContractDetails = PrjH.GetContractDetailList(strProjectCode, null);
                foreach (doProjectContractDetail i in doProjectContractDetails)
                {
                    if (i.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3198, new string[] { i.ContractCode_Short });
                        res.ResultData = true;
                        return Json(res);
                    }
                }
                //1.2.1
                string strProjectStatus = null;
                List<string> lstStatus = PrjH.GetProjectStatus(strProjectCode);
                if (lstStatus.Count > 0)
                    strProjectStatus = lstStatus[0];
                if (strProjectStatus == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3199);
                    res.ResultData = true;
                    return Json(res);
                }
                else if (strProjectStatus == ProjectStatus.C_PROJECT_STATUS_CANCEL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3200);
                    res.ResultData = true;
                    return Json(res);
                }
                return Json(false);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }


        }

        /// <summary>
        /// Validate business in process cancel
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_ValidateBusinessCancel(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            IProjectHandler PrjH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            try
            {
                List<doProjectContractDetail> doProjectContractDetails = PrjH.GetContractDetailList(strProjectCode, null);
                foreach (doProjectContractDetail i in doProjectContractDetails)
                {
                    if (i.ContractStatus != ContractStatus.C_CONTRACT_STATUS_CANCEL && i.ContractStatus != ContractStatus.C_CONTRACT_STATUS_END && i.ContractStatus != ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3201, new string[] { i.ContractCode_Short });
                        res.ResultData = true;
                        return Json(res);
                    }
                }
                string strProjectStatus = null;
                List<string> lstStatus = PrjH.GetProjectStatus(strProjectCode);
                if (lstStatus.Count > 0)
                    strProjectStatus = lstStatus[0];
                if (strProjectStatus == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3199);
                    res.ResultData = true;
                    return Json(res);
                }
                else if (strProjectStatus == ProjectStatus.C_PROJECT_STATUS_CANCEL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3200);
                    res.ResultData = true;
                    return Json(res);
                }
                res.ResultData = false;
                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); res.ResultData = true; return Json(res); }



        }
        #endregion
        #region Register
        /// <summary>
        /// Register last complete
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_RegisterLastComplete(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope Scope = new TransactionScope())
            {
                try
                {
                    
                    IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                    projH.UpdateProjectStatus(strProjectCode, ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE);

#if !ROUND1
                    IInventoryHandler iHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    bool blnProcessResult = iHand.UpdateCompleteProject(strProjectCode);
#endif

                    List<doTbt_Project> doTbt_Project = projH.GetTbt_ProjectForView(strProjectCode);
                    if (doTbt_Project.Count > 0)
                        res.ResultData = doTbt_Project[0].ProjectStatusCodeName;
                    Scope.Complete();

                }
                catch (Exception ex) { res.AddErrorMessage(ex); Json(res); }
            }

            return Json(res);
        }

        /// <summary>
        /// Register update to cancel status
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_RegisterCancel(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope Scope = new TransactionScope())
            {
                try
                {
                    IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                    projH.UpdateProjectStatus(strProjectCode, ProjectStatus.C_PROJECT_STATUS_CANCEL);

#if !ROUND1
                    IInventoryHandler iHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    bool blnProcessResult = iHand.UpdateCompleteProject(strProjectCode);
#endif
                    List<doTbt_Project> doTbt_Project = projH.GetTbt_ProjectForView(strProjectCode);
                    if (doTbt_Project.Count > 0)
                        res.ResultData = doTbt_Project[0].ProjectStatusCodeName;
                    Scope.Complete();
                }
                catch (Exception ex) { res.AddErrorMessage(ex); Json(res); }
            }
            return Json(res);
        }

        #endregion
        #region CheckBeforeAdd
        /// <summary>
        /// Validate before add system product
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_CheckBeforeAddSystemProduct(CTS240_SystemProduct Cond)
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
            if (param.doRegProject240 == null)
                param.doRegProject240 = new doRegisterProject240();
            if (param.doRegProject240.SystemProduct == null)
                param.doRegProject240.SystemProduct = new List<CTS240_SystemProduct>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                Cond.ActionFlag = ActionFlag.Insert;
                bool IsSysProdExists = false;
                if (!CommonUtil.IsNullOrEmpty(Cond.ProductCode))
                    for (int i = 0; i < param.doRegProject240.SystemProduct.Count; i++)
                    {
                        if (param.doRegProject240.SystemProduct[i].tmpProductCodeName == null)
                        {
                            param.doRegProject240.SystemProduct[i].tmpProductCodeName = param.doRegProject240.SystemProduct[i].ProductCodeName;
                        }
                        if (Cond.tmpProductCodeName == param.doRegProject240.SystemProduct[i].tmpProductCodeName)
                        {
                            if (param.doRegProject240.SystemProduct[i].ActionFlag == ActionFlag.Delete)
                            {
                                param.doRegProject240.SystemProduct.RemoveAt(i);
                                Cond.ActionFlag = ActionFlag.Edit;
                            }
                            else
                            {
                                IsSysProdExists = true;
                            }
                            break;
                        }
                    }
                if (IsSysProdExists)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3187, new string[] { Cond.tmpProductCodeName });
                    return Json(res);
                }
                param.doRegProject240.SystemProduct.Add(Cond);
                UpdateScreenObject(param);
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Delete system product 
        /// </summary>
        /// <param name="ProductCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_DelSystemProduct(string ProductCode)
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
            if (param.doRegProject240 == null)
                param.doRegProject240 = new doRegisterProject240();
            if (param.doRegProject240.SystemProduct == null)
                param.doRegProject240.SystemProduct = new List<CTS240_SystemProduct>();
            for (int i = 0; i < param.doRegProject240.SystemProduct.Count; i++)
            {
                if (param.doRegProject240.SystemProduct[i].ProductCode == ProductCode)
                {
                    if (param.doRegProject240.SystemProduct[i].ActionFlag == ActionFlag.Insert)
                        param.doRegProject240.SystemProduct.RemoveAt(i);
                    else
                        param.doRegProject240.SystemProduct[i].ActionFlag = ActionFlag.Delete;
                    break;
                }
            }
            UpdateScreenObject(param);
            return Json(true);
        }
        /// <summary>
        /// Validate before add other related data
        /// </summary>
        /// <param name="Cond"></param>
        /// <param name="lstCompanyName"></param>
        /// <param name="lstCompanyName_rowID"></param>
        /// <param name="row_id"></param>
        /// <returns></returns>
        public ActionResult CTS240_CheckBeforeAddOther(CTS240_OtherRelate Cond, string[] lstCompanyName, string[] lstCompanyName_rowID, string row_id)
        {
            ObjectResultData res = new ObjectResultData();
            if (ModelState.IsValid == false)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                    return Json(res);
            }
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.OtherRelate == null)
                    param.doRegProject240.OtherRelate = new List<CTS240_OtherRelate>();

                if (Cond.ActionFlag != ActionFlag.Edit)
                {
                    bool IsExist = false;
                    if (!CommonUtil.IsNullOrEmpty(Cond.CompanyName))
                        for (int i = 0; i < param.doRegProject240.OtherRelate.Count; i++)
                            if (Cond.CompanyName.ToUpper() == param.doRegProject240.OtherRelate[i].CompanyName.ToUpper())
                            {
                                if (param.doRegProject240.OtherRelate[i].ActionFlag == ActionFlag.Delete)
                                {
                                    param.doRegProject240.OtherRelate.RemoveAt(i);
                                    Cond.ActionFlag = ActionFlag.Edit;
                                }
                                else
                                {
                                    IsExist = true;
                                }
                                break;
                            }
                    if (IsExist)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3186, new string[] { Cond.CompanyName }, new string[] { "Company Name" });
                        return Json(res);
                    }
                    Cond.ActionFlag = ActionFlag.Insert;
                    param.doRegProject240.OtherRelate.Add(Cond);
                }
                else
                {
                    bool IsCompNameExist = false;
                    if (!CommonUtil.IsNullOrEmpty(lstCompanyName))
                    {
                        for (int i = 0; i < lstCompanyName.Length; i++)
                        {
                            if ((lstCompanyName[i].ToUpper() == Cond.CompanyName.ToUpper()) && row_id != lstCompanyName_rowID[i])
                            {
                                IsCompNameExist = true;
                                break;
                            }
                        }
                    }
                    if (IsCompNameExist)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3186, new string[] { Cond.CompanyName }, new string[] { "Company Name" });
                        return Json(res);
                    }

                    for (int i = 0; i < param.doRegProject240.OtherRelate.Count; i++)
                        if (Cond.SequenceNo == param.doRegProject240.OtherRelate[i].SequenceNo)
                        {
                            param.doRegProject240.OtherRelate[i].CompanyName = Cond.CompanyName;
                            param.doRegProject240.OtherRelate[i].Name = Cond.Name;
                            param.doRegProject240.OtherRelate[i].TelNo = Cond.TelNo;
                            param.doRegProject240.OtherRelate[i].Remark = Cond.Remark;

                            param.doRegProject240.OtherRelate[i].ActionFlag = ActionFlag.Edit;

                            break;
                        }
                }
                UpdateScreenObject(param);
                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }
        
        /// <summary>
        /// Validate before delete other related data
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_delOther(CTS240_OtherRelate Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.OtherRelate == null)
                    param.doRegProject240.OtherRelate = new List<CTS240_OtherRelate>();
                for (int i = 0; i < param.doRegProject240.OtherRelate.Count; i++)
                {
                    if (Cond.CompanyName.ToUpper() == param.doRegProject240.OtherRelate[i].CompanyName.ToUpper())
                    {
                        if (param.doRegProject240.OtherRelate[i].ActionFlag == ActionFlag.Insert)
                            param.doRegProject240.OtherRelate.RemoveAt(i);
                        else
                            param.doRegProject240.OtherRelate[i].ActionFlag = ActionFlag.Delete;
                        break;
                    }
                }
                res.ResultData = true;
                UpdateScreenObject(param);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
                return Json(res);
            }
        }
        
        /// <summary>
        /// Validate before add staff
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_CheckBeforeAddStaff(CTS240_SupportStaff Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.SupportStaff == null)
                    param.doRegProject240.SupportStaff = new List<CTS240_SupportStaff>();
                if (ModelState.IsValid == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this, null);
                    if (res.IsError)
                        return Json(res);
                }
                Cond.ActionFlag = ActionFlag.Insert;
                SupportStaff staff = getStaff(Cond.EmpNo, true);
                if (staff != null && CommonUtil.IsNullOrEmpty(staff.EmpFullName.Trim()))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { Cond.EmpNo }, new string[] { "SupportStaffCode" });
                    return Json(res);
                }
                bool IsExist = false;
                if (!CommonUtil.IsNullOrEmpty(Cond.EmpNo))
                    for (int i = 0; i < param.doRegProject240.SupportStaff.Count; i++)
                        if (Cond.EmpNo == param.doRegProject240.SupportStaff[i].EmpNo)
                        {
                            if (param.doRegProject240.SupportStaff[i].ActionFlag == ActionFlag.Delete)
                            {
                                param.doRegProject240.SupportStaff.RemoveAt(i);
                                Cond.ActionFlag = ActionFlag.Edit;
                            }
                            else
                            {
                                IsExist = true;
                            }
                            break;
                        }
                if (IsExist)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3188,new string [] {Cond.EmpNo});
                    return Json(res);
                }
                param.doRegProject240.SupportStaff.Add(Cond);
                UpdateScreenObject(param);
                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
                return Json(res);
            }

        }
        
        /// <summary>
        /// Delete support staff
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_delStaff(CTS240_SupportStaff Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.SupportStaff == null)
                    param.doRegProject240.SupportStaff = new List<CTS240_SupportStaff>();
                for (int i = 0; i < param.doRegProject240.SupportStaff.Count; i++)
                {
                    if (Cond.EmpNo.ToUpper() == param.doRegProject240.SupportStaff[i].EmpNo.ToUpper())
                    {
                        if (param.doRegProject240.SupportStaff[i].ActionFlag == ActionFlag.Insert)
                            param.doRegProject240.SupportStaff.RemoveAt(i);
                        else
                            param.doRegProject240.SupportStaff[i].ActionFlag = ActionFlag.Delete;
                        break;
                    }
                }
                res.ResultData = true;
                UpdateScreenObject(param);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
                return Json(res);
            }
        }
        
        /// <summary>
        /// Validate before add expect Instrument
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_CheckBeforeAddExpectInstrument(CTS240_ExpectInstrument Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.ExpectInstrument == null)
                    param.doRegProject240.ExpectInstrument = new List<CTS240_ExpectInstrument>();
                if (ModelState.IsValid == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this, null);
                    if (res.IsError)
                        return Json(res);
                }
                if ((!CommonUtil.IsNullOrEmpty(Cond.InstrumentCode)) && CommonUtil.IsNullOrEmpty(Cond.dtNewInstrument))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0082, new string[] { Cond.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                if (CommonUtil.IsNullOrEmpty(Cond.InstrumentQty) || Cond.InstrumentQty <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0084, null, new string[] { "InstrumentQty" });
                    return Json(res);
                }
                //======================= TRS Update 16/07/2012 ============================
                if (Cond.dtNewInstrument.InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0014, new string[] { Cond.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                if (Cond.dtNewInstrument.ExpansionTypeCode != ExpansionType.C_EXPANSION_TYPE_PARENT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0015, new string[] { Cond.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                if (Cond.dtNewInstrument.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0086, new string[] { Cond.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                //==========================================================================
                Cond.ActionFlag = ActionFlag.Insert;
                bool IsExist = false;
                if (!CommonUtil.IsNullOrEmpty(Cond.InstrumentCode))
                    for (int i = 0; i < param.doRegProject240.ExpectInstrument.Count; i++)
                        if (Cond.InstrumentCode.ToUpper() == param.doRegProject240.ExpectInstrument[i].InstrumentCode.ToUpper())
                        {
                            if (param.doRegProject240.ExpectInstrument[i].ActionFlag == ActionFlag.Delete)
                            {
                                //============= Teerapong S.16/08/2012 ====================
                                //param.doRegProject240.ExpectInstrument[i].ActionFlag = ActionFlag.Edit;
                                //param.doRegProject240.ExpectInstrument[i].InstrumentQty = Cond.InstrumentQty;
                            }
                            else
                            {
                                IsExist = true;
                            }
                            break;
                        }
                if (IsExist)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0083, new string[] { Cond.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }

                param.doRegProject240.ExpectInstrument.Add(Cond);
                UpdateScreenObject(param);
                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
                return Json(res);
            }

        }
        
        /// <summary>
        /// Delete Expect Instrument
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_delExpectInstrument(CTS240_ExpectInstrument Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.ExpectInstrument == null)
                    param.doRegProject240.ExpectInstrument = new List<CTS240_ExpectInstrument>();
                for (int i = 0; i < param.doRegProject240.ExpectInstrument.Count; i++)
                {
                    if (Cond.InstrumentCode.ToUpper() == param.doRegProject240.ExpectInstrument[i].InstrumentCode.ToUpper())
                    {
                        if (param.doRegProject240.ExpectInstrument[i].ActionFlag == ActionFlag.Insert)
                            param.doRegProject240.ExpectInstrument.RemoveAt(i);
                        else
                            param.doRegProject240.ExpectInstrument[i].ActionFlag = ActionFlag.Delete;
                        break;
                    }
                }
                res.ResultData = true;
                UpdateScreenObject(param);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
                return Json(res);
            }
        }


        //public ActionResult CTS240_AttachFile(HttpPostedFileBase uploadedFile, string DocName,
        // string action, string delID, string txtProjectCode, string Deleted)
        //{
        //    //Deleted = Deleted.Trim();
        //    //MessageModel MsgModel;
        //    //ObjectResultData res = new ObjectResultData();
        //    //ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //    //List<dtAttachFileNameID> lstAttachedName = handler.GetAttachFileName(txtProjectCode, null, null);

        //    //if (action == "delete" && delID != "" && (!CommonUtil.IsNullOrEmpty(txtProjectCode)))
        //    //{
        //    //    ICommonHandler comhand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //    //    comhand.RemoveAttachFile(AttachmentModule.Project, Convert.ToInt32(delID), txtProjectCode, null);
        //    //    List<dtAttachFileNameID> lstAttach = comhand.GetAttachFileName(txtProjectCode, null, null);
        //    //    ViewBag.AttachFileList = lstAttach;
        //    //    return View("CTS240_Upload");
        //    //}

        //    //if (lstAttachedName.Count >= AttachDocumentCondition.C_ATTACH_DOCUMENT_MAXIMUM_NUMBER && action == "upload")
        //    //{
        //    //    MsgModel = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0076);
        //    //    ViewBag.AttachFileList = lstAttachedName;
        //    //    ViewBag.Message = MsgModel.Message;
        //    //    ViewBag.MsgCode = "MSG0076";
        //    //    return View("CTS240_Upload");
        //    //}
        //    //else
        //    //{
        //    //    bool IsFileNameExist = false;
        //    //    foreach (dtAttachFileNameID i in lstAttachedName)
        //    //    {
        //    //        if (i.FileName.ToUpper() == DocName.ToUpper())
        //    //        {
        //    //            lstAttachedName = handler.GetAttachFileName(txtProjectCode, null, false);
        //    //            IsFileNameExist = true;
        //    //            break;
        //    //        }
        //    //    }
        //    //    if (IsFileNameExist)
        //    //    {
        //    //        MsgModel = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115);
        //    //        lstAttachedName = handler.GetAttachFileName(txtProjectCode, null, false);
        //    //        ViewBag.AttachFileList = lstAttachedName;
        //    //        ViewBag.Message = MsgModel.Message;
        //    //        ViewBag.MsgCode = "MSG0115";
        //    //        ViewBag.ProjectCode = txtProjectCode;
        //    //        return View("CTS240_Upload");
        //    //    }
        //    //}
        //    //if (uploadedFile != null)
        //    //{
        //    //    int fileSize = uploadedFile.ContentLength;
        //    //    if (uploadedFile.ContentLength > 0)
        //    //    {
        //    //        byte[] data;
        //    //        using (BinaryReader reader = new BinaryReader(uploadedFile.InputStream))
        //    //        {
        //    //            data = reader.ReadBytes(uploadedFile.ContentLength);
        //    //        }
        //    //        string fileType = Path.GetExtension(uploadedFile.FileName);
        //    //        List<tbt_AttachFile> attach = handler.InsertAttachFile(txtProjectCode,
        //    //                                        DocName,
        //    //                                          fileType,
        //    //                                          fileSize,
        //    //                                          data,
        //    //                                          false);
        //    //    }
        //    //}
        //    //lstAttachedName = handler.GetAttachFileName(txtProjectCode, null, null);
        //    //ViewBag.AttachFileList = lstAttachedName;
        //    //ViewBag.ProjectCode = txtProjectCode;
        //    return View("CTS240_Upload");
        //}
        #endregion


        #region Stock out Area
        /// <summary>
        /// Get stockout data
        /// </summary>
        /// <param name="BranchNo"></param>
        /// <returns></returns>
        public ActionResult GetStockOut(int? BranchNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> Stock = getParamStockOut();
                List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> aa = (from c in Stock where (c.BranchNo == BranchNo || BranchNo == null) select c).ToList<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                List<View_dtTbt_ProjectStockoutBranchIntrumentDetailForView> View_Stock = CommonUtil.ClonsObjectList<dtTbt_ProjectStockoutBranchIntrumentDetailForView, View_dtTbt_ProjectStockoutBranchIntrumentDetailForView>(aa);

                res.ResultData = CommonUtil.ConvertToXml<View_dtTbt_ProjectStockoutBranchIntrumentDetailForView>(View_Stock, "contract\\CTS240_stock", CommonUtil.GRID_EMPTY_TYPE.INSERT);
                //updateParamStockOut(param.doRegProject240.StockOut);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Update stockout data for view
        /// </summary>
        /// <param name="BranchNO"></param>
        /// <param name="InstrumentCode"></param>
        /// <param name="AssignQty"></param>
        /// <returns></returns>
        public ActionResult UpdateStockBranchForView(int? BranchNO, string InstrumentCode, int AssignQty)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> Stock = getParamStockOut();
                IEnumerable<dtTbt_ProjectStockoutBranchIntrumentDetailForView> qry = from STB in Stock where (STB.BranchNo == BranchNO && STB.InstrumentCode == InstrumentCode) select STB;
                List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> lstQry = qry.ToList<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                //Calculate assign branch qty
                if (lstQry != null && lstQry.Count > 0) //Add by Jutarat A. on 10072013
                {
                    if (AssignQty <= lstQry[0].SumNotAssign + lstQry[0].AssignBranchQty)
                    {
                        foreach (var item in qry)
                        {
                            item.SumAssignQty = (lstQry[0].SumAssignQty - lstQry[0].AssignBranchQty) + AssignQty;
                            item.SumNotAssign = (lstQry[0].SumNotAssign + lstQry[0].AssignBranchQty) - AssignQty;
                            item.AssignBranchQty = AssignQty;
                            if (item.ActionFlag != ActionFlag.Insert)
                                item.ActionFlag = ActionFlag.Edit;
                        }
                        updateParamStockOut(Stock);

                        List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> OtherStock = getParamStockOut();
                        IEnumerable<dtTbt_ProjectStockoutBranchIntrumentDetailForView> OtherQry = from STB in OtherStock where (STB.BranchNo != BranchNO && STB.InstrumentCode == InstrumentCode) select STB;
                        foreach (var item in OtherQry)
                        {

                            item.SumAssignQty = (lstQry[0].SumAssignQty - lstQry[0].AssignBranchQty) + AssignQty;
                            item.SumNotAssign = (lstQry[0].SumNotAssign + lstQry[0].AssignBranchQty) - AssignQty;
                        }
                        updateParamStockOut(OtherStock);
                    }
                }

                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
                return Json(res);
            }
        }
       
        /// <summary>
        /// Initial empty grid stock branch schema 
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_GetEmptyStockBranch()
        {
            ObjectResultData res = new ObjectResultData();
            res.ResultData = CommonUtil.ConvertToXml<View_dtTbt_ProjectStockoutBranchIntrumentDetailForView>(new List<View_dtTbt_ProjectStockoutBranchIntrumentDetailForView>(), "contract\\CTS240_stock", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }

        /// <summary>
        /// Update stockout from screen
        /// </summary>
        /// <param name="lst"></param>
        private void updateParamStockOut(List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> lst)
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
            param.doRegProject240.StockOut = lst;
            UpdateScreenObject(param);

        }
        private List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> getParamStockOut()
        {
            CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

            if (param.doRegProject240 == null)
                param.doRegProject240 = new doRegisterProject240();
            if (param.doRegProject240.StockOut == null)
                param.doRegProject240.StockOut = new List<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();

            //Add by Jutarat A. on 10072013 (Default New Branch No.)
            if (param.doRegProject240.StockOut.Count == 0)
            {
                if (param.lstWip != null && param.lstWip.Count > 0)
                {
                    param.doRegProject240.StockOut = CommonUtil.ClonsObjectList<dtTbt_ProjectStockoutIntrumentForView, dtTbt_ProjectStockoutBranchIntrumentDetailForView>(param.lstWip);
                    foreach (dtTbt_ProjectStockoutBranchIntrumentDetailForView data in param.doRegProject240.StockOut)
                    {
                        data.BranchNo = 1;
                        data.AssignBranchQty = 0;
                        data.SumNotAssign = data.InstrumentQty;
                        data.ActionFlag = ActionFlag.Insert;
                    }
                }
            }
            //End Add

            return param.doRegProject240.StockOut;
        }

        #endregion

        /// <summary>
        /// Validate before register
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS240_RegisterCommand(doRegisterProject240 Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();
                if (param.doRegProject240.doRegCust == null)
                    param.doRegProject240.doRegCust = new doRegisterCustomer();

                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_CHANGE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }

                tbt_ProjectPurchaserCustomer tbt_Purchaser = CommonUtil.CloneObject<doRegisterCustomer, tbt_ProjectPurchaserCustomer>(Cond.doRegCust);
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                ValidatorUtil.BuildErrorMessage(res, new object[] { tbt_Purchaser, Cond.doTbt_Project }, null, false);
                if (res.IsError)
                    return Json(res);

                CTS230_doValidateEmpNo EmpError = CTS240_ValidateEmployee(Cond.doTbt_Project);
                if (EmpError.isError)
                {

                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { EmpError.EmpNo }, new string[] { EmpError.controls });
                    return Json(res);
                }

                #region Line-up type

                if (param.doRegProject240.ExpectInstrument != null)
                {
                    foreach (CTS240_ExpectInstrument inst in param.doRegProject240.ExpectInstrument)
                    {
                        if (inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE
                            || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_ONE_TIME
                            || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_TEMPORARY)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3296);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                            break;
                        }
                    }
                }

                #endregion

                Cond.doTbt_Project.ProjectCode = param.strProjectCode;
                Cond.doTbt_Project.ProjectStatus = ProjectStatus.C_PROJECT_STATUS_PROCESSING;

                IProjectHandler PrjH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<tbt_Project> lstOldPrj = PrjH.GetTbt_Project(param.strProjectCode);
                Cond.doTbt_Project.ReceivedInstallationFee = lstOldPrj[0].ReceivedInstallationFee;
                Cond.doTbt_Project.ReceivedInstrumentPrice = lstOldPrj[0].ReceivedInstrumentPrice;
                Cond.doTbt_Project.StockedOutInstrumentAmount = lstOldPrj[0].StockedOutInstrumentAmount;

                Cond.doTbt_Project.InstallationOrderedAmount = lstOldPrj[0].InstallationOrderedAmount;
                param.doRegProject240.doTbt_Project = Cond.doTbt_Project;
                param.doRegProject240.doRegCust = Cond.doRegCust;
                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
                return Json(res);
            }

        }

        /// <summary>
        /// validate confirm and call register data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_ConfirmCommand()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {       //Check Suspend

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_CHANGE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }


                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                if (param != null && param.doRegProject240 != null && param.doRegProject240.doRegCust != null)
                {
                    RegisterCorrectData(param.doRegProject240, param.strProjectCode);
                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                    List<tbt_AttachFile> tmpFileList = comHand.GetTbt_AttachFile(param.strProjectCode, null, false);
                    if (tmpFileList != null && tmpFileList.Count > 0)
                    {
                        comHand.UpdateFlagAttachFile(AttachmentModule.Project, param.strProjectCode, param.strProjectCode);
                    }
                    return Json(true);
                }
                else
                {

                    return Json(false);
                }
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }


        /// <summary>
        /// Register data
        /// </summary>
        /// <param name="Cond"></param>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        private string RegisterCorrectData(doRegisterProject240 Cond, string strProjectCode)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    CTS240_ScreenParameter prm = GetScreenObject<CTS240_ScreenParameter>();
                    IProjectHandler PrjH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

                    #region Expect

                    IEnumerable<CTS240_ExpectInstrument> qry = from c in Cond.ExpectInstrument where c.ActionFlag == ActionFlag.Delete select c;
                    List<tbt_ProjectExpectedInstrumentDetails> lstExpect = CommonUtil.ClonsObjectList<CTS240_ExpectInstrument, tbt_ProjectExpectedInstrumentDetails>(qry.ToList<CTS240_ExpectInstrument>());
                    foreach (tbt_ProjectExpectedInstrumentDetails i in lstExpect)
                        PrjH.DeleteTbt_ProjectExpectedInstrumentDetails(strProjectCode, i.InstrumentCode);

                    qry = from c in Cond.ExpectInstrument where c.ActionFlag == ActionFlag.Edit select c;
                    lstExpect = CommonUtil.ClonsObjectList<CTS240_ExpectInstrument, tbt_ProjectExpectedInstrumentDetails>(qry.ToList<CTS240_ExpectInstrument>());
                    foreach (tbt_ProjectExpectedInstrumentDetails i in lstExpect)
                        PrjH.UpdateTbt_ProjectExpectedInstrumentDetails(strProjectCode, i.InstrumentCode, i.InstrumentQty);

                    qry = from c in Cond.ExpectInstrument where (c.ActionFlag == ActionFlag.Insert) select c;
                    lstExpect = CommonUtil.ClonsObjectList<CTS240_ExpectInstrument, tbt_ProjectExpectedInstrumentDetails>(qry.ToList<CTS240_ExpectInstrument>());
                    PrjH.InsertTbt_ProjectExpectedInstrumentDetail(lstExpect);

                    #endregion
                    #region System

                    IEnumerable<CTS240_SystemProduct> qrySys = from c in Cond.SystemProduct where c.ActionFlag == ActionFlag.Delete select c;
                    List<tbt_ProjectSystemDetails> lstSys = CommonUtil.ClonsObjectList<CTS240_SystemProduct, tbt_ProjectSystemDetails>(qrySys.ToList<CTS240_SystemProduct>());
                    foreach (tbt_ProjectSystemDetails i in lstSys)
                        PrjH.DeleteTbt_ProjectSystemDetails(strProjectCode, i.ProductCode);

                    qrySys = from c in Cond.SystemProduct where (c.ActionFlag == ActionFlag.Insert) select c;
                    lstSys = CommonUtil.ClonsObjectList<CTS240_SystemProduct, tbt_ProjectSystemDetails>(qrySys.ToList<CTS240_SystemProduct>());
                    PrjH.InsertTbt_ProjectSystemDetail(lstSys);



                    #endregion
                    #region Support

                    IEnumerable<CTS240_SupportStaff> qrySupport = from c in Cond.SupportStaff where c.ActionFlag == ActionFlag.Delete select c;
                    List<tbt_ProjectSupportStaffDetails> lstSupport = CommonUtil.ClonsObjectList<CTS240_SupportStaff, tbt_ProjectSupportStaffDetails>(qrySupport.ToList<CTS240_SupportStaff>());
                    foreach (tbt_ProjectSupportStaffDetails i in lstSupport)
                        PrjH.DeleteTbt_ProjectSupportStaffDetails(strProjectCode, i.EmpNo);


                    qrySupport = from c in Cond.SupportStaff where (c.ActionFlag == ActionFlag.Insert) select c;
                    lstSupport = CommonUtil.ClonsObjectList<CTS240_SupportStaff, tbt_ProjectSupportStaffDetails>(qrySupport.ToList<CTS240_SupportStaff>());

                    PrjH.InsertTbt_ProjectSupportStaffDetail(lstSupport);

                    #endregion
                    #region Other
                    IEnumerable<CTS240_OtherRelate> qryOther = from c in Cond.OtherRelate where c.ActionFlag == ActionFlag.Delete select c;
                    List<tbt_ProjectOtherRalatedCompany> lstOther = CommonUtil.ClonsObjectList<CTS240_OtherRelate, tbt_ProjectOtherRalatedCompany>(qryOther.ToList<CTS240_OtherRelate>());
                    foreach (tbt_ProjectOtherRalatedCompany i in lstOther)
                        PrjH.DeleteTbt_ProjectOtherRalatedCompany(strProjectCode, i.SequenceNo);

                    qryOther = from c in Cond.OtherRelate where c.ActionFlag == ActionFlag.Edit select c;
                    lstOther = CommonUtil.ClonsObjectList<CTS240_OtherRelate, tbt_ProjectOtherRalatedCompany>(qryOther.ToList<CTS240_OtherRelate>());
                    foreach (tbt_ProjectOtherRalatedCompany i in lstOther)
                        PrjH.UpdateTbt_ProjectOtherRalatedCompany(strProjectCode, i.SequenceNo, i.CompanyName, i.Name, i.TelNo, i.Remark);

                    qryOther = from c in Cond.OtherRelate where (c.ActionFlag == ActionFlag.Insert) select c;
                    lstOther = CommonUtil.ClonsObjectList<CTS240_OtherRelate, tbt_ProjectOtherRalatedCompany>(qryOther.ToList<CTS240_OtherRelate>());

                    PrjH.InsertTbt_ProjectOtherRalatedCompany(lstOther);



                    #endregion
                    #region StockBranch
                    List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> qryStock = (from c in Cond.StockOut where (c.ActionFlag == ActionFlag.Edit) select c).ToList<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                    List<tbt_ProjectStockoutBranchIntrumentDetails> lstTbtStockBranch = new List<tbt_ProjectStockoutBranchIntrumentDetails>();
                    if (qryStock.Count > 0)
                    {
                        foreach (dtTbt_ProjectStockoutBranchIntrumentDetailForView i in qryStock)
                            PrjH.UpdateTbt_ProjectStockoutBranchIntrumentDetails(strProjectCode, i.BranchNo, (int)i.AssignBranchQty, i.InstrumentCode);
                            //PrjH.UpdateTbt_ProjectStockoutBranchIntrumentDetails(strProjectCode, i.BranchNo, (int)i.AssignBranchQty);
                    }

                    qryStock = (from c in Cond.StockOut where (c.ActionFlag == ActionFlag.Insert) select c).ToList<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                    lstTbtStockBranch = CommonUtil.ClonsObjectList<dtTbt_ProjectStockoutBranchIntrumentDetailForView, tbt_ProjectStockoutBranchIntrumentDetails>(qryStock);

                    PrjH.InsertTbt_ProjectStockoutBranchIntrumentDetails(lstTbtStockBranch);
                    #endregion

                    if (Cond.doTbt_Project.OverallBudgetAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        Cond.doTbt_Project.OverallBudgetAmountUsd = Cond.doTbt_Project.OverallBudgetAmount;
                        Cond.doTbt_Project.OverallBudgetAmount = null;
                    }
                    else
                    {
                        Cond.doTbt_Project.OverallBudgetAmountUsd = null;
                    }

                    if (Cond.doTbt_Project.ReceivedBudgetAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        Cond.doTbt_Project.ReceivedBudgetAmountUsd = Cond.doTbt_Project.ReceivedBudgetAmount;
                        Cond.doTbt_Project.ReceivedBudgetAmount = null;
                    }
                    else
                    {
                        Cond.doTbt_Project.ReceivedBudgetAmountUsd = null;
                    }

                    if (Cond.doTbt_Project.LastOrderAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        Cond.doTbt_Project.LastOrderAmountUsd = Cond.doTbt_Project.LastOrderAmount;
                        Cond.doTbt_Project.LastOrderAmount = null;
                    }
                    else
                    {
                        Cond.doTbt_Project.LastOrderAmountUsd = null;
                    }
                    
                    PrjH.UpdateTbt_ProjectData(Cond.doTbt_Project);

                    //if (Cond.doRegCust.ActionFlag == ActionFlag.Edit)// Comment by Jutarat A. on 16072013 (Not Use)
                    //{
                        tbt_ProjectPurchaserCustomer Purchaser = CommonUtil.CloneObject<doRegisterCustomer, tbt_ProjectPurchaserCustomer>(Cond.doRegCust);
                        Purchaser.ProjectCode = prm.strProjectCode;

                        //Modify by Jutarat A. on 16072013
                        //PrjH.UpdateTbt_ProjectPurchaseCustomer(Purchaser);
                        List<dtTbt_ProjectPurchaserCustomerForView> ProjectPurchaserCustomerList = PrjH.GetTbt_ProjectPurchaserCustomerForView(prm.strProjectCode);
                        if (ProjectPurchaserCustomerList != null && ProjectPurchaserCustomerList.Count > 0)
                        {
                            PrjH.UpdateTbt_ProjectPurchaseCustomer(Purchaser);
                        }
                        else
                        {
                            PrjH.InsertTbt_ProjectPurchaserCustomer(Purchaser);
                        }
                        //End Modify
                    //}

                    //ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    //comh.UpdateFlagAttachFile(AttachmentModule.Project, strProjectCode, strProjectCode);


                    scope.Complete();
                    return null;
                }
                catch { throw; }
            }
        }

        /// <summary>
        /// Reset all session
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_ResetCommand(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //comh.RemoveAttachFile(AttachmentModule.Project, null, strProjectCode, false);

                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();

                if (param.doRegProject240 == null)
                    param.doRegProject240 = new doRegisterProject240();

                param.doRegProject240.doRegCust = new doRegisterCustomer();
                param.doRegProject240.doTbt_Project = new CTS240_ProjectData();
                param.doRegProject240.ExpectInstrument = new List<CTS240_ExpectInstrument>();
                param.doRegProject240.StockOut = new List<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                param.doRegProject240.SupportStaff = new List<CTS240_SupportStaff>();
                param.doRegProject240.SystemProduct = new List<CTS240_SystemProduct>();
                param.doRegProject240.OtherRelate = new List<CTS240_OtherRelate>();

                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            //return View();
        }

        /// <summary>
        /// Get new branch no
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_NewBranchNo()
        {
            IProjectHandler PrjH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS240_ScreenParameter prm = GetScreenObject<CTS240_ScreenParameter>();
                
                //if (prm.doRegProject240.StockOut == null || prm.doRegProject240.StockOut.Count <= 0)
                if (prm.lstWip == null || prm.lstWip.Count == 0) //Modify by Jutarat A. on 10072013
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3240);
                    return Json(res);
                }
                if (!CommonUtil.IsNullOrEmpty(prm.strProjectCode) && prm.doRegProject240 != null && prm.doRegProject240.StockOut != null && prm.doRegProject240.StockOut.Count > 0)
                {
                    List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> lstNewStockBranch = new List<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
                    List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> lstStockOut = (from c in prm.doRegProject240.StockOut where (c.BranchNo == (prm.doRegProject240.StockOut.Max(a => a.BranchNo))) select c).ToList();
                    int NewBranchNo = prm.doRegProject240.StockOut.Max<dtTbt_ProjectStockoutBranchIntrumentDetailForView>(a => a.BranchNo) + 1;
                    bool canCreate = false;
                    foreach (dtTbt_ProjectStockoutBranchIntrumentDetailForView i in prm.doRegProject240.StockOut)
                        if (i.SumNotAssign > 0)
                        {
                            canCreate = true;
                            break;
                        }
                    if (canCreate == false)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3241);
                        return Json(res);
                    }


                    foreach (dtTbt_ProjectStockoutBranchIntrumentDetailForView i in lstStockOut)
                    {
                        dtTbt_ProjectStockoutBranchIntrumentDetailForView NewStockBranch = new dtTbt_ProjectStockoutBranchIntrumentDetailForView();
                        NewStockBranch.ActionFlag = ActionFlag.Insert;
                        NewStockBranch.AssignBranchQty = 0;
                        NewStockBranch.BranchNo = NewBranchNo;
                        NewStockBranch.InstrumentCode = i.InstrumentCode;
                        NewStockBranch.InstrumentName = i.InstrumentName;
                        NewStockBranch.SumAssignQty = i.SumAssignQty;
                        NewStockBranch.SumNotAssign = i.SumNotAssign;
                        NewStockBranch.InstrumentQty = i.InstrumentQty;
                        NewStockBranch.ProjectCode = prm.strProjectCode;
                        prm.doRegProject240.StockOut.Add(NewStockBranch);
                    }
                    res.ResultData = NewBranchNo;
                    return Json(res);
                }
                else
                {
                    //res.ResultData = false;
                    res.ResultData = 1; //Modify by Jutarat A. on 10072013 (Default New Branch No.)
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
        /// Update instrument qty
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <param name="InstrumentQty"></param>
        /// <returns></returns>
        public ActionResult CTS240_UpdateInstrumentQty(string InstrumentCode, int InstrumentQty)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS240_ScreenParameter prm = GetScreenObject<CTS240_ScreenParameter>();

                List<CTS240_ExpectInstrument> ExpectInstrument = (from c in prm.doRegProject240.ExpectInstrument where (c.InstrumentCode == InstrumentCode && c.InstrumentQty != InstrumentQty) select c).ToList();
                if (ExpectInstrument.Count > 0)
                {
                    ExpectInstrument[0].InstrumentQty = InstrumentQty;
                    ExpectInstrument[0].ActionFlag = ActionFlag.Edit;
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Validate employee data
        /// </summary>
        /// <param name="Project"></param>
        /// <returns></returns>
        public CTS230_doValidateEmpNo CTS240_ValidateEmployee(CTS240_ProjectData Project)
        {
            ObjectResultData res = new ObjectResultData();

            List<tbm_Employee> dtEmp = new List<tbm_Employee>();
            if (!CommonUtil.IsNullOrEmpty(Project.HeadSalesmanEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.HeadSalesmanEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.ProjectManagerEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.ProjectManagerEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.ProjectSubManagerEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.ProjectSubManagerEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.SecurityPlanningChiefEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.SecurityPlanningChiefEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.InstallationChiefEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.InstallationChiefEmpNo;
                dtEmp.Add(tmp);
            }
            CTS230_doValidateEmpNo EmpError = new CTS230_doValidateEmpNo();
            IEmployeeMasterHandler EmpH = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            List<doActiveEmployeeList> dtTbm_Employee = EmpH.GetActiveEmployeeList(dtEmp);
            string EmpNo;
            string Controls;

            string Prefix = "st";
            if (dtTbm_Employee.Count != dtEmp.Count)
            {
                if (!CommonUtil.IsNullOrEmpty(Project.HeadSalesmanEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.HeadSalesmanEmpNo))
                {
                    EmpNo = Project.HeadSalesmanEmpNo;
                    Controls = Prefix + "HeadSalesmanEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.ProjectManagerEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.ProjectManagerEmpNo))
                {
                    EmpNo = Project.ProjectManagerEmpNo;
                    Controls = Prefix + "ProjectManagerEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.ProjectSubManagerEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.ProjectSubManagerEmpNo))
                {
                    EmpNo = Project.ProjectSubManagerEmpNo;
                    Controls = Prefix + "ProjectSubManagerEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.SecurityPlanningChiefEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.SecurityPlanningChiefEmpNo))
                {
                    EmpNo = Project.SecurityPlanningChiefEmpNo;
                    Controls = Prefix + "SecurityPlanningChiefEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.InstallationChiefEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.InstallationChiefEmpNo))
                {
                    EmpNo = Project.InstallationChiefEmpNo;
                    Controls = Prefix + "InstallationChiefEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
            }
            return new CTS230_doValidateEmpNo();
        }
       
        /// <summary>
        /// Get data stockout memo
        /// </summary>
        /// <param name="ProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_GetStockOutMemo(string ProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                CTS240_ScreenParameter param = GetScreenObject<CTS240_ScreenParameter>();
                List<tbt_ProjectStockOutMemo> StockOutMemo = projH.GetTbt_ProjectStockoutMemoForView(ProjectCode);
                string Memo = "";


                for (int i = StockOutMemo.Count - 1; i >= 0; i--)
                {
                    Memo = Memo + "Stock-out: " + StockOutMemo[i].Sequence + "   Stock-out by: " + StockOutMemo[i].CreateByName +
                    "   Stock-out date: " + string.Format("{0:dd-MMM-yyyy}", Convert.ToDateTime(StockOutMemo[i].CreateDate)) + "\n" + StockOutMemo[i].Memo + "\n\n";
                }

                res.ResultData = Memo;
                return Json(res);


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }


        /// <summary>
        /// Generate branch combobox
        /// </summary>
        /// <param name="ProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS240_RegenBranchCombo(string ProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<doProjectBranch> lstBranch;
                lstBranch = projH.GetProjectStockOutBranch(ProjectCode);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doProjectBranch>(lstBranch, "BranchNo", "ProjectCodeBranch", true);

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Attach file
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="ReferKey"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult CTS240_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string ReferKey,string k)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            MessageModel outmsg = null;

            try
            {
                byte[] fileData;
                if (fileSelect == null)
                {
                    outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0050, null);
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(fileSelect.InputStream))
                    {
                        var fList = commonhandler.GetAttachFileForGridView(ReferKey);

                        var filterDupItem = from a in fList where a.FileName.ToUpper().Equals(DocumentName.ToUpper() + Path.GetExtension(fileSelect.FileName).ToUpper()) select a;

                        if (filterDupItem.Count() > 0)
                        {
                            // Docname duplicate
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                        }
                        else if (DocumentName == null || DocumentName == "")
                        {
                            string nparam = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS300", "lblDocumentName");
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { nparam });
                        }
                        else
                        {
                            fileData = reader.ReadBytes(fileSelect.ContentLength);

                            if (commonhandler.CanAttachFile(DocumentName, fileData.Length, Path.GetExtension(fileSelect.FileName), ReferKey, GetCurrentKey()))
                            {
                                DateTime currDate = DateTime.Now;
                                commonhandler.InsertAttachFile(ReferKey
                                , DocumentName
                                , Path.GetExtension(fileSelect.FileName)
                                , fileData.Length
                                , fileData
                                , false);
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                outmsg = new MessageModel();
                outmsg.Message = ((SECOM_AJIS.Common.Models.ApplicationErrorException)(ex)).ErrorResult.Message.Message;
                outmsg.Code = CommonValue.SYSTEM_MESSAGE_CODE;
            }

            if (outmsg != null)
            {
                ViewBag.Message = outmsg.Message;
                ViewBag.MsgCode = outmsg.Code;
            }
            ViewBag.K = k;
            ViewBag.ReferKey = ReferKey;
            return View("CTS240_Upload");
        }

        /// <summary>
        /// Initial grid attach
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS240_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS240_attach", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Remove attach
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS240_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                int _attachID = int.Parse(AttachID);
                CTS240_ScreenParameter sParam = GetScreenObject<CTS240_ScreenParameter>();
                //commonhandler.DeleteAttachFileByID(_attachID, Session.SessionID);
                commonhandler.DeleteAttachFileByID(_attachID, sParam.strProjectCode);
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Load data to grid attach
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public ActionResult CTS240_LoadGridAttachedDocList(string test)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS240_ScreenParameter sParam = GetScreenObject<CTS240_ScreenParameter>();
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(sParam.strProjectCode);
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Contract\\CTS240_attach", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /* Old Code
        /// <summary>
        /// Download attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS240_DownloadAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS240_ScreenParameter sParam = GetScreenObject<CTS240_ScreenParameter>();
                var downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), sParam.strProjectCode);
                var downloadFileName = commonhandler.GetTbt_AttachFile(sParam.strProjectCode, int.Parse(AttachID), null);
                //var downloadFileName = commonhandler.GetAttachFileName(sParam.strIncidentID, int.Parse(AttachID), null);
                //var attachFile = commonhandler.GetAttachFile(AttachmentModule.Incident, ReleateID, int.Parse(AttachID));
                //var fileNameLst = commonhandler.GetAttachFileName(ReleateID, int.Parse(AttachID), true);

                string fileName = downloadFileName[0].FileName;
                return File(downloadFileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }*/

        //Add by Jutarat A. on 31012013
        /// <summary>
        /// Download attach file
        /// </summary>
        /// <param name="AttachID"></param>
        public void CTS240_DownloadAttach(string AttachID)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            CTS240_ScreenParameter sParam = GetScreenObject<CTS240_ScreenParameter>();

            Stream downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), sParam.strProjectCode);
            List<tbt_AttachFile> downloadFileName = commonhandler.GetTbt_AttachFile(sParam.strProjectCode, int.Parse(AttachID), null);

            this.DownloadAllFile(downloadFileName[0].FileName, downloadFileStream);
        }
        //End Add

        /// <summary>
        /// Initial upload attach section
        /// </summary>
        /// <param name="k"></param>
        /// <param name="sK"></param>
        /// <returns></returns>
        public ActionResult CTS240_SendAttachKey(string k,string sK = "")
        {
            ViewBag.ReferKey = sK;
            ViewBag.K = k;
            return View("CTS240_Upload");
        }


    }

}

