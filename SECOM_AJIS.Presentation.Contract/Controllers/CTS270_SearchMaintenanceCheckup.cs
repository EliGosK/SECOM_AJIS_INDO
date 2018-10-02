//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract.CustomEntity;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS270_Authority(CTS270_ScreenParameter CTS270Param)
        {
            ObjectResultData res = new ObjectResultData();

            //1. Check system suspending
            res = checkSuspending();
            if (res.IsError)
                return Json(res);

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_VIEW))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            //CTS270_ScreenParameter CTS270Param = new CTS270_ScreenParameter();
            CTS270Param.hasRegisterPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_ADD);
            CTS270Param.hasDeletePermision = CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_DEL);
            CTS270Param.hasSlipDownloadPermision = CheckUserPermission(DocumentCode.C_DOCUMENT_CODE_MAINTENANCE_CHECKUP_SLIP, FunctionID.C_FUNC_ID_DOWNLOAD);
            CTS270Param.hasListDownloadPermision = CheckUserPermission(DocumentCode.C_DOCUMENT_CODE_MAINTENANCE_CHECKUP_LIST, FunctionID.C_FUNC_ID_DOWNLOAD);
            if (CTS270Param.hasSlipDownloadPermision || CTS270Param.hasListDownloadPermision)
                CTS270Param.hasDownloadButton = true;

            return InitialScreenEnvironment<CTS270_ScreenParameter>("CTS270", CTS270Param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS270")]
        public ActionResult CTS270()
        {
            CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>();

            //Clear data if not return from child page
            if (CTS270Param.CallerScreenID != ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP)
            {
                CTS270Param.data = null;
            }
            
            dsCTS270Data data = CTS270Param.data;
            if (data != null)
            {
                //Convert result to current language
                CommonUtil.MappingObjectLanguage<dtSearchMACheckupResult>(data.dtSearchResult);

                //Clear isBtnClick flag to clear session after come back from child page
                CTS270Param.isBtnClick = false;
                
                ViewBag.MaintenanceCheckupSlipFlag = data.doSearchCondition.MaintenanceCheckupSlipFlag;
                ViewBag.MaintenanceCheckupListFlag = data.doSearchCondition.MaintenanceCheckupListFlag;
                ViewBag.RelatedContractType = data.doSearchCondition.RelatedContractType ?? "0";
                ViewBag.OperationOffice = (data.doSearchCondition.OperationOffice == null || data.doSearchCondition.OperationOffice.Contains(",")) ? "" : data.doSearchCondition.OperationOffice;
                ViewBag.ProductName = data.doSearchCondition.ProductName ?? "";
                ViewBag.CheckupInstructionMonthFrom = data.doSearchCondition.CheckupInstructionMonthFrom;
                ViewBag.CheckupInstructionYearFrom = data.doSearchCondition.CheckupInstructionYearFrom;
                ViewBag.CheckupInstructionMonthTo = data.doSearchCondition.CheckupInstructionMonthTo;
                ViewBag.CheckupInstructionYearTo = data.doSearchCondition.CheckupInstructionYearTo;
                ViewBag.SiteName = data.doSearchCondition.SiteName ?? "";
                ViewBag.UserCodeContractCode = data.doSearchCondition.UserCodeContractCode ?? "";
                ViewBag.MAEmployeeName = data.doSearchCondition.MAEmployeeName ?? "";
                ViewBag.MACheckupNo = data.doSearchCondition.MACheckupNo ?? "";
                ViewBag.HasCheckupResult = data.doSearchCondition.HasCheckupResult;
                ViewBag.HaveInstrumentMalfunction = data.doSearchCondition.HaveInstrumentMalfunction;
                ViewBag.NeedToContactSalesman = data.doSearchCondition.NeedToContactSalesman;
                ViewBag.CurrentIndex = CTS270Param.CurrentIndex;
                ViewBag.CurrentSortColIndex = CTS270Param.CurrentSortColIndex;
                ViewBag.CurrentSortType = CTS270Param.CurrentSortType;
                ViewBag.HasSessionData = true;
            }
            else
            {
                if (CTS270Param.hasSlipDownloadPermision)
                    ViewBag.MaintenanceCheckupSlipFlag = true;

                if (!CTS270Param.hasSlipDownloadPermision && CTS270Param.hasListDownloadPermision)
                    ViewBag.MaintenanceCheckupListFlag = true;

                ViewBag.CheckupInstructionMonthFrom = DateTime.Now.Month;
                ViewBag.CheckupInstructionYearFrom = DateTime.Now.Year;
                ViewBag.CheckupInstructionMonthTo = DateTime.Now.Month;
                ViewBag.CheckupInstructionYearTo = DateTime.Now.Year;
                ViewBag.CurrentIndex = 0;
                ViewBag.HasSessionData = false;
                ViewBag.RelatedContractType = "0";
            }

            ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_SEARCHPAGE;
            ViewBag.CallerPage = CTS270Param.CallerScreenID;
            ViewBag.ViewLabel = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_SEARCH_MAINTENANCE_CHECKUP, "btnView");
            ViewBag.RegisterLabel = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_SEARCH_MAINTENANCE_CHECKUP, "btnRegister");
            ViewBag.DeleteLabel = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_SEARCH_MAINTENANCE_CHECKUP, "btnDelete");
            ViewBag.FunctionIdView = FunctionID.C_FUNC_ID_VIEW;
            ViewBag.FunctionIdAdd = FunctionID.C_FUNC_ID_ADD;
            ViewBag.HasSlipRdoPermission = CTS270Param.hasSlipDownloadPermision;
            ViewBag.HasListRdoPermission = CTS270Param.hasListDownloadPermision;
            ViewBag.HasRegisterPermission = CTS270Param.hasRegisterPermission;
            ViewBag.HasDeletePermision = CTS270Param.hasDeletePermision;
            ViewBag.HasDownloadButton = CTS270Param.hasDownloadButton;
            // Akat K. for enable download when has permission download list
            ViewBag.HasDownloadListPermission = CTS270Param.hasListDownloadPermision;
            ViewBag.HasDownloadSlipPermission = CTS270Param.hasSlipDownloadPermision;

            UpdateScreenObject(CTS270Param);

            return View("CTS270");
        }

        #endregion

        /// <summary>
        /// Initial grid
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CTS270()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS270", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Check required field
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS270_CheckReqField(CTS270_CheckReqField cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                //Validate time from to
                DateTime dateFrom = new DateTime().AddDays(0).AddMonths(cond.CheckupInstructionMonthFrom.Value - 1).AddYears(cond.CheckupInstructionYearFrom.Value - 1);
                DateTime dateTo = new DateTime().AddDays(0).AddMonths(cond.CheckupInstructionMonthTo.Value - 1).AddYears(cond.CheckupInstructionYearTo.Value - 1);
                if (dateFrom > dateTo)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0062
                        , new String[] { "Check-up instruction (From)", "Check-up instruction (To)" }
                        , new String[] { "CheckupInstructionMonthFrom", "CheckupInstructionYearFrom", "CheckupInstructionMonthTo", "CheckupInstructionYearTo" });
                    return Json(res);
                }

                //Clear data in session when click on search button
                CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>();
                CTS270Param.data = null;
                UpdateScreenObject(CTS270Param); //clear data
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load search result data to grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS270_Search(doSearchMACheckupCriteria cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>();
                dsCTS270Data data = CTS270Param.data;
                List<dtSearchMACheckupResult> list = new List<dtSearchMACheckupResult>();

                if (data == null)
                {
                    //Save search condition value
                    dsCTS270Data dsData = new dsCTS270Data();
                    cond.HasCheckupResult = cond.HasCheckupResult ?? false;
                    cond.HaveInstrumentMalfunction = cond.HaveInstrumentMalfunction ?? false;
                    cond.NeedToContactSalesman = cond.NeedToContactSalesman ?? false;
                    dsData.doSearchCondition = cond;

                    //Set default to some search condition
                    CommonUtil c = new CommonUtil();
                    cond.UserCode = cond.UserCodeContractCode;
                    cond.ContractCode = c.ConvertContractCode(cond.UserCodeContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    //cond.MACheckupNo = (cond.MACheckupNo == null) ? null : System.Text.RegularExpressions.Regex.Replace(cond.MACheckupNo, "^0+", "");
                    
                    if (cond.OperationOffice == null)
                    {
                        List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                        StringBuilder sbOperationOffice = new StringBuilder("");
                        foreach (OfficeDataDo off in clst)
                        {
                            if (off.FunctionSecurity != SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity.C_FUNC_SECURITY_NO)// filter follow by the combobox
                                sbOperationOffice.AppendFormat("\'{0}\',", off.OfficeCode);
                        }
                        cond.OperationOffice = sbOperationOffice.ToString();
                    }
                    
                    //Query for maintenance
                    IMaintenanceHandler hand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                    list = hand.SearchMACheckup(cond);

                    //Set disable/show Action Buttons
                    setPermissionFlag(list
                        , (cond.MaintenanceCheckupListFlag.HasValue) ? cond.MaintenanceCheckupListFlag.Value : false
                        , (cond.MaintenanceCheckupSlipFlag.HasValue) ? cond.MaintenanceCheckupSlipFlag.Value : false
                        , CTS270Param);

                    //Save search result list
                    dsData.dtSearchResult = list;
                    
                    //Save condition and result into session
                    CTS270Param.data = dsData;
                    UpdateScreenObject(CTS270Param);
                }
                else
                {
                    list = data.dtSearchResult;
                    
                    //Set disable/show Action Buttons
                    setPermissionFlag(list
                        , data.doSearchCondition.MaintenanceCheckupListFlag ?? false
                        , data.doSearchCondition.MaintenanceCheckupSlipFlag ?? false
                        , CTS270Param);
                }

                res.ResultData = CommonUtil.ConvertToXml<dtSearchMACheckupResult>(list, "Contract\\CTS270", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);

            }
            return Json(res);
        }
        /// <summary>
        /// Set permission flag
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isMaintCheckupListFlag"></param>
        /// <param name="isMaintCheckupSlipFlag"></param>
        /// <param name="CTS270Param"></param>
        public void setPermissionFlag(List<dtSearchMACheckupResult> list, bool isMaintCheckupListFlag, bool isMaintCheckupSlipFlag, CTS270_ScreenParameter CTS270Param)
        {
            bool hasRegisterPermission = CTS270Param.hasRegisterPermission;
            bool hasDeletePermision = CTS270Param.hasDeletePermision;
            
            foreach (dtSearchMACheckupResult item in list)
            {
                //Download Checkbox Permission
                

                //Select/Unselect Button Permission
                //if (isMaintCheckupSlipFlag)
                //    item.EnableSelectButtonFlag = true;
                //else
                //    item.EnableSelectButtonFlag = false;

                bool hasChekupNo = !CommonUtil.IsNullOrEmpty(item.CheckupNo);
                bool hasMADate = !CommonUtil.IsNullOrEmpty(item.MaintenanceDate);
                bool isInstDatePass = (int.Parse(item.InstructionDate.ToString("yyyyMM")) <= int.Parse(DateTime.Now.AddMonths(1).ToString("yyyyMM")));

                item.EnableCheckboxFlag = "0";
                item.EnableViewFlag = "0";
                item.EnableRegisterFlag = "0";
                item.EnableDeleteFlag = "0";

                if (hasChekupNo == true)
                {
                    if (hasMADate == true)
                        item.EnableViewFlag = "1";
                    else
                    {
                        if (hasRegisterPermission && isInstDatePass)
                            item.EnableRegisterFlag = "1";

                        if (hasDeletePermision)
                            item.EnableDeleteFlag = "1";
                    }
                }
                else 
                {
                    if (isMaintCheckupSlipFlag && isInstDatePass)
                        item.EnableCheckboxFlag = "1";

                    if (hasDeletePermision)
                        item.EnableDeleteFlag = "1";
                }
            }
        }
        /// <summary>
        /// Check system is suspending
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS270_CheckSystemSuspending()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);
                else
                    res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Delete selected data from session
        /// </summary>
        /// <param name="DeleteItem"></param>
        /// <param name="CheckItemList"></param>
        /// <returns></returns>
        public ActionResult CTS270_Delete(CTS270_CheckResultItem DeleteItem, List<CTS270_CheckResultItem> CheckItemList)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //Check System Suspending
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                //Validate business
                //Cannot delete maintenance schedule if maintenance check-up schedule of maintenance contract that has many products and there is some registered results in the same cycle
                IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;

                bool isSomeResultRegistered = mainHand.IsSomeResultRegistered(DeleteItem.ContractCode, DeleteItem.InstructionDate.Value);
                if (isSomeResultRegistered)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3193);
                    return Json(res);
                }

                //Delete data from search result list in session
                CTS270_ScreenParameter CTS270Param = GetScreenObject <CTS270_ScreenParameter>();
                var objects = from l in CTS270Param.data.dtSearchResult
                              where l.ContractCode == DeleteItem.ContractCode
                              && l.ProductCode == DeleteItem.ProductCode
                              && l.InstructionDate == DeleteItem.InstructionDate.Value
                              select l;

                foreach (dtSearchMACheckupResult obj in objects)
                {
                    //Delete maintainence checkup schedule
                    mainHand.DeleteMaintenanceCheckupSchedule(DeleteItem.ContractCode, DeleteItem.ProductCode, DeleteItem.InstructionDate.Value, obj.UpdateDate.Value);
                    
                    //Remove data from session
                    CTS270Param.data.dtSearchResult.Remove(obj);

                    if (CheckItemList != null)
                    {
                        List<dtSearchMACheckupResult> nSort = new List<dtSearchMACheckupResult>();
                        foreach (CTS270_CheckResultItem ma in CheckItemList)
                        {
                            foreach (dtSearchMACheckupResult r in CTS270Param.data.dtSearchResult)
                            {
                                if (r.KeyIndex == ma.KeyIndex)
                                {
                                    if (ma.CheckedFlag == true)
                                        r.CheckedFlag = "1";

                                    nSort.Add(r);
                                    break;
                                }
                            }
                        }
                        CTS270Param.data.dtSearchResult = nSort;
                    }

                    UpdateScreenObject(CTS270Param);
                    break;
                }
               
                //when finish with out error
                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0047);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load result data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS270_GetResultList()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>();
                dsCTS270Data data = CTS270Param.data;
                res.ResultData = CommonUtil.ConvertToXml<dtSearchMACheckupResult>(data.dtSearchResult, "Contract\\CTS270", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);

            }
            return Json(res);
        }
        /// <summary>
        /// Download data
        /// </summary>
        /// <param name="bChkListFlag"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public ActionResult CTS270_Download(bool bChkListFlag, List<CTS270_CheckResultItem> list)
        {
            ObjectResultData res = new ObjectResultData();
            IReportHandler rptHandler;

            try
            {
                rptHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;

                CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>();
                CTS270Param.csvRpt = null;
                CTS270Param.pdfRpt = null;

                List<tbt_MaintenanceCheckup> nList = new List<tbt_MaintenanceCheckup>();
                if (bChkListFlag == true)
                {
                    foreach (dtSearchMACheckupResult r in CTS270Param.data.dtSearchResult)
                    {
                        nList.Add(new tbt_MaintenanceCheckup()
                        {
                            ContractCode = r.ContractCode,
                            ProductCode = r.ProductCode,
                            InstructionDate = r.InstructionDate
                        });
                    }
                }
                else
                {
                    foreach (CTS270_CheckResultItem l in list)
                    {
                        if (l.CheckedFlag == true)
                        {
                            foreach (dtSearchMACheckupResult r in CTS270Param.data.dtSearchResult)
                            {
                                if (l.KeyIndex == r.KeyIndex)
                                {
                                    nList.Add(new tbt_MaintenanceCheckup()
                                    {
                                        ContractCode = r.ContractCode,
                                        ProductCode = r.ProductCode,
                                        InstructionDate = r.InstructionDate
                                    });

                                    break;
                                }
                            }
                        }
                    }
                }

                IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                if (bChkListFlag)
                {
                    //string csv = mainHand.GenerateMACheckupList(list);
                    string csv = rptHandler.GetMaintenanceCheckupList(nList);
                    if (!CommonUtil.IsNullOrEmpty(csv))
                    {
                        CTS270Param.csvRpt = csv;
                    }
                    
                    //return File(csv, "text/csv");
                }
                else
                {
                    bool isEmptyList = true;
                    if (nList != null)
                    {
                        if (nList.Count > 0)
                            isEmptyList = false;
                    }
                    if (isEmptyList == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0017);
                        return Json(res);
                    }
                    else
                    {
                        doMACheckupSlipResult rm = mainHand.GenerateMACheckupSlip(nList);
                        if (!CommonUtil.IsNullOrEmpty(rm.ResultData))
                        {
                            //Stream result = new MemoryStream(merge);
                            CTS270Param.pdfRpt = rm.ResultData;
                        }

                        List<dtSearchMACheckupResult> nSort = new List<dtSearchMACheckupResult>();
                        foreach (CTS270_CheckResultItem ma in list)
                        {
                            string CheckupNo = null;
                            foreach (tbt_MaintenanceCheckup n in nList)
                            {
                                if (ma.KeyIndex == n.KeyIndex)
                                {
                                    CheckupNo = n.CheckupNo;
                                    break;
                                }
                            }
                            foreach (dtSearchMACheckupResult r in CTS270Param.data.dtSearchResult)
                            {
                                if (r.KeyIndex == ma.KeyIndex)
                                {
                                    if (CommonUtil.IsNullOrEmpty(CheckupNo) == false)
                                    {
                                        r.CheckedFlag = "";
                                        r.CheckupNo = CheckupNo;
                                    }
                                    else if (ma.CheckedFlag == true)
                                    {
                                        r.CheckedFlag = "1";
                                    }
                                    else
                                    {
                                        r.CheckedFlag = "";
                                    }

                                    nSort.Add(r);
                                    break;
                                }
                            }
                        }
                        CTS270Param.data.dtSearchResult = nSort;

                        //Set disable/show Action Buttons
                        setPermissionFlag(CTS270Param.data.dtSearchResult
                            , CTS270Param.data.doSearchCondition.MaintenanceCheckupListFlag ?? false
                            , CTS270Param.data.doSearchCondition.MaintenanceCheckupSlipFlag ?? false
                            , CTS270Param);

                        //return File(result, "application/pdf");

                        if (rm.Error != null)
                        {
                            res.ResultData = new object[]{
                                rm.Error.ErrorResult.Message,
                                rm.ErrorDetail
                            };
                        }
                    }
                }

                UpdateScreenObject(CTS270Param);
                if (res.ResultData == null)
                    res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get download result file
        /// </summary>
        public void CTS270_DownloadSubmit() //(string strSessionKey)
        {
            CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>(); //(strSessionKey);
            if (CTS270Param.csvRpt != null)
            {
                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + "MACheckupList.csv");
                //Response.ContentType = "text/csv"; //"application/force-download";
                //Response.Charset = "windows-874";
                //Response.ContentEncoding = System.Text.Encoding.GetEncoding(874);
                //Response.Write(CTS270Param.csvRpt);
                //Response.End();
                this.DownloadCSVFile("MACheckupList.csv", CTS270Param.csvRpt);
            }
            else if (CTS270Param.pdfRpt != null)
            {
                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + "MACheckupSlip.pdf");
                //Response.ContentType = "application/pdf"; 
                //Response.Charset = "UTF-8";
                //Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                ////Response.Write(CTS270Param.pdfRpt);
                //Response.BinaryWrite(CTS270Param.pdfRpt);
                //Response.End();
                this.DownloadPDFFile("MACheckupSlip.pdf", CTS270Param.pdfRpt);
            }
        }
        /// <summary>
        /// Check system is suspending
        /// </summary>
        /// <returns></returns>
        private ObjectResultData checkSystemSuspending()
        {
            ObjectResultData res = new ObjectResultData();

            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bSuspendingStatus = commonHandler.IsSystemSuspending();

            if (bSuspendingStatus)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
            }

            return res;
        }
        /// <summary>
        /// Set button status
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="CurrentIndex"></param>
        /// <param name="list"></param>
        /// <param name="CurrentSortColIndex"></param>
        /// <param name="CurrentSortType"></param>
        /// <returns></returns>
        public ActionResult CTS270_SetBtnClickFlag(string Mode, int CurrentIndex, List<CTS270_CheckResultItem> list, int CurrentSortColIndex, string CurrentSortType)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (Mode == FunctionID.C_FUNC_ID_ADD)
                {
                    //Check System Suspending
                    res = checkSystemSuspending();
                    if (res.IsError)
                        return Json(res);
                }

                CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>();
                CTS270Param.isBtnClick = true;
                CTS270Param.CurrentIndex = CurrentIndex;
                CTS270Param.CurrentSortColIndex = CurrentSortColIndex;
                CTS270Param.CurrentSortType = CurrentSortType;

                List<dtSearchMACheckupResult> nSort = new List<dtSearchMACheckupResult>();
                foreach (CTS270_CheckResultItem ma in list)
                {
                    foreach (dtSearchMACheckupResult r in CTS270Param.data.dtSearchResult)
                    {
                        if (r.KeyIndex == ma.KeyIndex)
                        {
                            r.CheckedFlag = ma.CheckedFlag ? "1" : "";
                            nSort.Add(r);
                            break;
                        }
                    }
                }
                CTS270Param.data.dtSearchResult = nSort;

                UpdateScreenObject(CTS270Param);
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Clear data from session
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS270_ClearSession()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>();
                if (!CTS270Param.isBtnClick)
                    UpdateScreenObject(null);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Update selected data in session
        /// </summary>
        /// <param name="callerSessionKey"></param>
        /// <param name="contractCode"></param>
        /// <param name="productCode"></param>
        /// <param name="instructionDate"></param>
        /// <param name="maintDate"></param>
        /// <param name="expectedMaintDate"></param>
        public void CTS270_UpdateDataFromChildPage(string callerSessionKey, string contractCode, string productCode, DateTime? instructionDate, DateTime? maintDate, DateTime? expectedMaintDate)
        {
            CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>(callerSessionKey);
            if (CTS270Param.data != null)
            {
                var objects = from l in CTS270Param.data.dtSearchResult
                              where l.ContractCode == contractCode
                              && l.ProductCode == productCode
                              && l.InstructionDate == instructionDate.Value
                              select l;

                foreach (dtSearchMACheckupResult obj in objects)
                {
                    obj.MaintenanceDate = maintDate;
                    obj.ExpectedMaintenanceDate = expectedMaintDate;
                }

                CTS270Param.IsLoaded = false;

                ScreenParameter oparam = (ScreenParameter)GetScreenObject<object>();
                if (oparam != null)
                {
                    CTS270Param.CallerScreenID = oparam.ScreenID;
                    CTS270Param.CallerModule = oparam.Module;
                    CTS270Param.CallerKey = oparam.Key;
                    CTS270Param.BackStep = true;
                }

                UpdateScreenObject(CTS270Param, callerSessionKey);
            }
        }
       
        //public ActionResult CTS270_MultiGenerateGrid()
        //{
        //    return Json(new ObjectResultData());
        //}
    }
}
