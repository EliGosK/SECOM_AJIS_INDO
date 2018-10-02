
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Transactions;
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
using SECOM_AJIS.Presentation.Contract.Models;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {

        #region AuthorityAndInitial
        /// <summary>
        /// Authority screen CTS260
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS260_Authority(CTS260_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_VIEW))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                //Valid Cond

                //if (CommonUtil.IsNullOrEmpty(param.strProjectCode))
                //{
                //    if (!CommonUtil.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ProjectCode))
                //    {
                //        param.strProjectCode = CommonUtil.dsTransData.dtCommonSearch.ProjectCode;
                //    }
                //}
                if (param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ProjectCode) == false)
                        param.strProjectCode = param.CommonSearch.ProjectCode;
                }

                //ValidatorUtil.BuildErrorMessage(res, this, null);
                //if (res.IsError)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                //    return Json(res);
                //}
                //if (CommonUtil.IsNullOrEmpty(param.strProjectCode))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3216, new string[] { "lblProjectCode" });
                //    return Json(res);
                //}

                //IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                //List<doTbt_Project> lstProject = Projh.GetTbt_ProjectForView(param.strProjectCode);
                //if (lstProject.Count <= 0)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, new string[] { param.strProjectCode });
                //    return Json(res);
                //}
                if (CommonUtil.IsNullOrEmpty(param.strProjectCode) == false)
                {
                    IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                    List<doTbt_Project> doTbt_Project = projH.GetTbt_ProjectForView(param.strProjectCode);
                    if (doTbt_Project.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, new string[] { param.strProjectCode });
                        return Json(res);
                    }
                }

                // param.strProjectCode = "p0000132";
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS260_ScreenParameter>("CTS260", param, res);

        }

        /// <summary>
        /// Initial screen CTS260
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS260")]
        public ActionResult CTS260()
        {
            CTS260_ScreenParameter param = GetScreenObject<CTS260_ScreenParameter>();
            if (param != null)
            {
                ViewBag.strProjectCode = param.strProjectCode;                
            }


            IProjectHandler ProjH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            List<tbt_ProjectStockoutBranchIntrumentDetails> lstBranch;
            lstBranch = ProjH.GetTbt_ProjectStockoutBranchIntrumentDetails(param.strProjectCode);
            if (lstBranch.Count > 0)
                ViewBag.BranchDDL = lstBranch[0].ProjectCodeBranchNo;
            return View();

        }


        #endregion
        #region initGrid
        /// <summary>
        /// Initial grid other
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_Other()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS260_Other"));
        }
        /// <summary>
        /// Injitial grid support
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_support()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS260_support"));
        }
        /// <summary>
        /// Initial grid expect
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_expect()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS260_expect"));
        }

        /// <summary>
        /// Initial grid system
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_system()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS260_system"));
        }

        /// <summary>
        /// Initial grid stock
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_stock()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS260_stock"));
        }

        /// <summary>
        /// Initial grid wip
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_wip()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS260_wip"));
        }

        /// <summary>
        /// Initial grid attach
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_attach()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS260_attach"));
        }

        #endregion
        /// <summary>
        /// Get project purchaser data
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS260_GetProjectPurchaser(string strProjectCode)
        {
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
            return Json(lstPurchaser);
        }

        /// <summary>
        /// Get project data for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS260_GetProjectForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            
            if (CommonUtil.IsNullOrEmpty(strProjectCode))
            {
                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS260",
                    MessageUtil.MODULE_COMMON,
                    MessageUtil.MessageList.MSG0007,
                    new string[] { "lblProjectCode" },
                    new string[] { "ProjectCode" });
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                return Json(res);
            }

            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            List<doTbt_Project> lstProject = Projh.GetTbt_ProjectForView(strProjectCode);
            if (lstProject.Count <= 0)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091,
                        new string[] { strProjectCode },
                        new string[] { "ProjectCode" });
                return Json(res);
            }
            else
            {
                //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = strProjectCode;
                CommonUtil comUtil = new CommonUtil();

                //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = comUtil.ConvertProjectCode(lstProject[0].ProjectCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;
                CTS260_ScreenParameter param = GetScreenObject<CTS260_ScreenParameter>();
                param.CommonSearch = new ScreenParameter.CommonSearchDo()
                {
                    ProjectCode = comUtil.ConvertProjectCode(lstProject[0].ProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT)
                };
            }
            EmployeeMappingList emlst = new EmployeeMappingList();
            emlst.AddEmployee(lstProject.ToArray());
            IEmployeeMasterHandler Emph = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            Emph.EmployeeListMapping(emlst);

            MiscTypeMappingList miscMapList = new MiscTypeMappingList();
            miscMapList.AddMiscType(lstProject.ToArray());
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            comh.MiscTypeMappingList(miscMapList);

            return Json(lstProject);
        }

        /// <summary>
        /// Get support staff for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS260_GetSupportStaffForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            List<dtTbt_ProjectSupportStaffDetailForView> lstStaff = Projh.GetTbt_ProjectSupportStaffDetailForView(strProjectCode);
            EmployeeMappingList emlst = new EmployeeMappingList();
            emlst.AddEmployee(lstStaff.ToArray());
            IEmployeeMasterHandler Emph = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            Emph.EmployeeListMapping(emlst);
            string result = CommonUtil.ConvertToXml<dtTbt_ProjectSupportStaffDetailForView>(lstStaff, "contract\\CTS260_support", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(result);
        }
        /// <summary>
        /// Get expect instrument data for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS260_GetExpectIntrumentDetailForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            List<dtTbt_ProjectExpectedInstrumentDetailsForView> lstIntrument = Projh.GetTbt_ProjectExpectedInstrumentDetailsForView(strProjectCode);

            string result = CommonUtil.ConvertToXml<dtTbt_ProjectExpectedInstrumentDetailsForView>(lstIntrument, "contract\\CTS260_expect", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(result);
        }
        /// <summary>
        /// Get system detial data for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS260_GetSystemDetailForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            List<dtTbt_ProjectSystemDetailForView> lstSysProd = Projh.GetTbt_ProjectSystemDetailForView(strProjectCode);

            string result = CommonUtil.ConvertToXml<dtTbt_ProjectSystemDetailForView>(lstSysProd, "contract\\CTS260_system", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(result);
        }
        /// <summary>
        /// Get project WIP data for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS260_ProjectWIPForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            List<dtTbt_ProjectStockoutIntrumentForView> lstWip = Projh.GetTbt_ProjectStockoutIntrumentForView(strProjectCode);
            string result = CommonUtil.ConvertToXml<dtTbt_ProjectStockoutIntrumentForView>(lstWip, "contract\\CTS260_wip", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(result);
        }
        /// <summary>
        /// Get other related data for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS260_OtherRelateForView(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            IProjectHandler Projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            List<tbt_ProjectOtherRalatedCompany> lstOther = Projh.GetTbt_ProjectOtherRalatedCompanyForView(strProjectCode);
            string result = CommonUtil.ConvertToXml<tbt_ProjectOtherRalatedCompany>(lstOther, "contract\\CTS260_other", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(result);
        }
        /// <summary>
        /// Get data stockout for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="BranchNo"></param>
        /// <returns></returns>
        public ActionResult CTS260_GetStockOutForView(string strProjectCode, int? BranchNo)
        {
            IProjectHandler projh = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> lstBranch;
            if (!CommonUtil.IsNullOrEmpty(strProjectCode) && !CommonUtil.IsNullOrEmpty(BranchNo))
                lstBranch = projh.GetTbt_ProjectStockoutBranchIntrumentDetailForView(strProjectCode, BranchNo);
            else
                lstBranch = new List<dtTbt_ProjectStockoutBranchIntrumentDetailForView>();
            string res = CommonUtil.ConvertToXml<dtTbt_ProjectStockoutBranchIntrumentDetailForView>(lstBranch, "contract\\CTS260_stock", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }
        
        public ActionResult CTS260_Download(string ReleateID, int AttachFileID)
        {
            //ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            //Stream FileBinary = comh.GetAttachFile(AttachmentModule.Project, ReleateID, AttachFileID);
            //List<dtAttachFileNameID> lstFileName = comh.GetAttachFileName(ReleateID, AttachFileID, true);
            //string FileType = "";
            //if (lstFileName.Count > 0)
            //    FileType = lstFileName[0].FileType.Substring(0);

            //return File(FileBinary, "application/" + FileType, lstFileName[0].FileName);
            return View();
        }
        public ActionResult CTS260_AttachGrid(string strProjectCode)
        {
            //ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            //List<dtAttachFileNameID> lstFileName = new List<dtAttachFileNameID>();
            //lstFileName = comh.GetAttachFileName(strProjectCode, null, true);
            //string attachGrid;
            //// if(lstFileName.Count>0)
            //attachGrid = CommonUtil.ConvertToXml<dtAttachFileNameID>(lstFileName, "contract\\CTS260_attach", CommonUtil.GRID_EMPTY_TYPE.VIEW);

            //return Json(attachGrid);
            return View();
        }       
        /// <summary>
        /// Initial grid attach document
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS260_attach", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// load attach file to grid attach
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS260_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS260_ScreenParameter param = GetScreenObject<CTS260_ScreenParameter>();
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(param.strProjectCode);
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Contract\\CTS260_attach", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        public ActionResult CTS260_DownloadAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS260_ScreenParameter param = GetScreenObject<CTS260_ScreenParameter>();
                var downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), param.strProjectCode);
                var downloadFileName = commonhandler.GetTbt_AttachFile(param.strProjectCode, int.Parse(AttachID), null);
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
        public void CTS260_DownloadAttach(string AttachID)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            CTS260_ScreenParameter param = GetScreenObject<CTS260_ScreenParameter>();

            Stream downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), param.strProjectCode);
            List<tbt_AttachFile> downloadFileName = commonhandler.GetTbt_AttachFile(param.strProjectCode, int.Parse(AttachID), null);

            this.DownloadAllFile(downloadFileName[0].FileName, downloadFileStream);
        }
        //End Add

    }
}