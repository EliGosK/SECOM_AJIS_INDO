
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS180
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS180_Authority(CMS180_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_INSTALLATION, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // If param.ContractCode is null then set to  CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                //if (CommonUtil.IsNullOrEmpty(param.ContractCode) == true)
                //{
                //    param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                //}
                param.CommonSearch = new ScreenParameter.CommonSearchDo()
                {
                    ContractCode = param.ContractCode
                };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS180_ScreenParameter>("CMS180", param, res);
        }

        /// <summary>
        /// Initialize screen of CMS180
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS180")]
        public ActionResult CMS180()
        {
            ObjectResultData res = new ObjectResultData();
           
            try
            {
                CMS180_ScreenParameter param = GetScreenObject<CMS180_ScreenParameter>();
                //for test
                //param.ContractCode = "N2700112";
                //param.InstallationSlipNo = "70000120120101";

                //Prepare for show section
                if (!CommonUtil.IsNullOrEmpty(param))
                {
                    ViewBag.ContractCode = param.ContractCode;
                    ViewBag.InstallationSlipNo = param.InstallationSlipNo;
                }


                if (string.IsNullOrEmpty(param.ContractCode) == false)
                {
                    //Finding service type code 
                    CommonUtil c = new CommonUtil();
                    string longContractCode = c.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                    //Rental
                    IRentralContractHandler handlerR = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    List<tbt_RentalContractBasic> dtRentalContract = handlerR.GetTbt_RentalContractBasic(longContractCode, null);
                    if (dtRentalContract.Count > 0)
                    {
                        param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    }
                    else
                    {
                        // Sale
                        ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        List<tbt_SaleBasic> dtSaleContract = handlerS.GetTbt_SaleBasic(longContractCode, null, true);
                        if (dtSaleContract.Count > 0)
                        {
                            param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                        }
                    }
                }
                ViewBag.ServiceTypeCode = param.ServiceTypeCode;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            //ViewBag.AttachKey = GetCurrentKey();
            return View();
        }

        /// <summary>
        /// Initial grid of view installation
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS180_InitialResultinstallation()
        {
            return Json(CommonUtil.ConvertToXml<dtInstallation>(null, "Common\\CMS180_ViewInstallationInfo", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid of sub contractor
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS180_InitialSubcontractor()
        {
            return Json(CommonUtil.ConvertToXml<dtInstallationPOManagementForView>(null, "Common\\CMS180_SubContractor", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// initial grid of instrument detail
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS180_InitialInstrumentDetail()
        {
            return Json(CommonUtil.ConvertToXml<dtInstallationSlipDetailsForView>(null, "Common\\CMS180_InstrumentDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search and get installation data list for view with initial
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS180_SearchResultWithInitial()
        {
            CMS180_ScreenParameter param = GetScreenObject<CMS180_ScreenParameter>();
            CommonUtil cm = new CommonUtil();
            ObjectResultData res = new ObjectResultData();
            List<dtInstallation> lst = new List<dtInstallation>();
            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                doSearchInstallationCondition cond = new doSearchInstallationCondition();

                if (!CommonUtil.IsNullOrEmpty(param.ContractCode) || !CommonUtil.IsNullOrEmpty(param.InstallationSlipNo))
                {
                    cond.ContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    cond.slipNo = param.InstallationSlipNo;
                    cond.ViewFlag = true;
                    cond.InstallationBy = null;
                }
                else 
                {
                    cond.InstallationBy = InstallationBy.C_INSTALLATION_BY_SECOM;
                }
                lst = handler.GetInstallationDataListForView(cond);
                res.ResultData = CommonUtil.ConvertToXml<dtInstallation>(lst, "Common\\CMS180_ViewInstallationInfo", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Search and get installation data list for view
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS180_SearchResult(doSearchInstallationCondition cond)
        {
            CMS180_ScreenParameter param = GetScreenObject<CMS180_ScreenParameter>();
            CommonUtil cm = new CommonUtil();
            ObjectResultData res = new ObjectResultData();
            List<dtInstallation> lst = new List<dtInstallation>();
            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;


                // Concate string CustomerTypeCode with commar separate. like ,xx,yy,zz, 
                List<string> lstManagementStatus = new List<string>();
                lstManagementStatus.Add(cond.chkInstallationNotRegistered);
                lstManagementStatus.Add(cond.chkInstallationRequestedAndPoRegistered);
                lstManagementStatus.Add(cond.chkInstallationNotRequest);
                lstManagementStatus.Add(cond.chkInstallationUnderInstall); //Add by Jutarat A. on 27032014
                lstManagementStatus.Add(cond.chkInstallationCompleted);
                lstManagementStatus.Add(cond.chkInstallationRequestButPoNotRegistered);
                lstManagementStatus.Add(cond.chkInstallationCancelled);
                cond.installationStatus = CommonUtil.CreateCSVString(lstManagementStatus);

                List<string> lstSlipStatus = new List<string>();
                lstSlipStatus.Add(cond.chkNotStockOut);
                lstSlipStatus.Add(cond.chkNoNeedToStockOut);
                lstSlipStatus.Add(cond.chkReturned);
                lstSlipStatus.Add(cond.chkPartialStockOut);
                lstSlipStatus.Add(cond.chkInstallationSlipCanceled);
                lstSlipStatus.Add(cond.chkNoNeedToReturn);
                lstSlipStatus.Add(cond.chkStockOut);
                lstSlipStatus.Add(cond.chkWaitForReturn);
                lstSlipStatus.Add(cond.chkReplaced);
                cond.slipStatus = CommonUtil.CreateCSVString(lstSlipStatus);

                List<string> lstInstallationManagementStatus = new List<string>();
                lstInstallationManagementStatus.Add(cond.chkProcessing);
                lstInstallationManagementStatus.Add(cond.chkApproved);
                lstInstallationManagementStatus.Add(cond.chkCompleted);
                lstInstallationManagementStatus.Add(cond.chkRequestApprove);
                lstInstallationManagementStatus.Add(cond.chkRejected);
                lstInstallationManagementStatus.Add(cond.chkCanceled);
                cond.managementStatus = CommonUtil.CreateCSVString(lstInstallationManagementStatus);

                cond.siteCode = cm.ConvertSiteCode(cond.siteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                cond.ViewFlag = false;
                cond.ContractCode = cm.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                
                List<string> ignoreList = new List<string>();

                ignoreList.Add("chkInstallationNotRegistered");
                ignoreList.Add("chkInstallationRequestedAndPoRegistered");
                ignoreList.Add("chkInstallationNotRequest");
                ignoreList.Add("chkInstallationUnderInstall"); //Add by Jutarat A. on 27032014
                ignoreList.Add("chkInstallationCompleted");
                ignoreList.Add("chkInstallationRequestButPoNotRegistered");
                ignoreList.Add("chkInstallationCancelled");

                ignoreList.Add("chkNotStockOut");
                ignoreList.Add("chkNoNeedToStockOut");
                ignoreList.Add("chkReturned");
                ignoreList.Add("chkPartialStockOut");
                ignoreList.Add("chkInstallationSlipCanceled");
                ignoreList.Add("chkNoNeedToReturn");
                ignoreList.Add("chkStockOut");
                ignoreList.Add("chkWaitForReturn");
                ignoreList.Add("chkReplaced");


                ignoreList.Add("chkProcessing");
                ignoreList.Add("chkApproved");
                ignoreList.Add("chkCompleted");
                ignoreList.Add("chkRequestApprove");
                ignoreList.Add("chkRejected");
                ignoreList.Add("chkCanceled");


                ignoreList.Add("ViewFlag");
                ignoreList.Add("slipNoNullFlag");
                //================ Teerapong S. 24/09/2011 ================
                ignoreList.Add("NotRegisteredYetSlipFlag");
                ignoreList.Add("NotRegisteredYetManagementFlag");
                //=========================================================


                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //=============== Teerapong S. 24/09/2011 ==================
                if (
                (CommonUtil.IsNullOrEmpty(cond.chkInstallationNotRegistered) && CommonUtil.IsNullOrEmpty(cond.chkInstallationRequestedAndPoRegistered) && CommonUtil.IsNullOrEmpty(cond.chkInstallationNotRequest) && CommonUtil.IsNullOrEmpty(cond.chkInstallationCompleted) && CommonUtil.IsNullOrEmpty(cond.chkInstallationRequestButPoNotRegistered) && CommonUtil.IsNullOrEmpty(cond.chkInstallationCancelled)) && CommonUtil.IsNullOrEmpty(cond.chkInstallationUnderInstall) || //Add (chkInstallationUnderInstall) by Jutarat A. on 27032014)
                (CommonUtil.IsNullOrEmpty(cond.chkNotStockOut) && CommonUtil.IsNullOrEmpty(cond.chkNoNeedToStockOut) && CommonUtil.IsNullOrEmpty(cond.chkReturned) && CommonUtil.IsNullOrEmpty(cond.chkPartialStockOut) && CommonUtil.IsNullOrEmpty(cond.chkInstallationSlipCanceled) && CommonUtil.IsNullOrEmpty(cond.chkNoNeedToReturn) && CommonUtil.IsNullOrEmpty(cond.chkStockOut) && CommonUtil.IsNullOrEmpty(cond.chkWaitForReturn) && CommonUtil.IsNullOrEmpty(cond.chkReplaced) && CommonUtil.IsNullOrEmpty(cond.NotRegisteredYetSlipFlag)) ||
                (CommonUtil.IsNullOrEmpty(cond.chkProcessing) && CommonUtil.IsNullOrEmpty(cond.chkApproved) && CommonUtil.IsNullOrEmpty(cond.chkCompleted) && CommonUtil.IsNullOrEmpty(cond.chkRequestApprove) && CommonUtil.IsNullOrEmpty(cond.chkRejected) && CommonUtil.IsNullOrEmpty(cond.chkCanceled) && CommonUtil.IsNullOrEmpty(cond.NotRegisteredYetManagementFlag)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0158);
                    return Json(res);
                }
                //=========================================================
                if (CommonUtil.IsNullAllField(cond, ignoreList.ToArray()))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                    return Json(res);
                }
                if (cond.slipIssueDateFrom > cond.slipIssueDateTo)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0090);
                    return Json(res);
                }
                cond.InstallationBy = InstallationBy.C_INSTALLATION_BY_SECOM;
                lst = handler.GetInstallationDataListForView(cond);

                res.ResultData = CommonUtil.ConvertToXml<dtInstallation>(lst, "Common\\CMS180_ViewInstallationInfo", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Getting installation history
        /// </summary>
        /// <param name="ContractProjectCode"></param>
        /// <param name="MaintenanceNo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CMS180_InstallationHistory(string ContractProjectCode, string MaintenanceNo,string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //IInstallationHandler hand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                //List<tbt_InstallationHistory> lst = hand.GetTbt_InstallationHistory(ContractProjectCode, MaintenanceNo, null);
                IInstallationHandler hand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                
                //List<dtInstallationHistoryForView> lst = hand.GetTbt_InstallationHistoryForView(
                //                                             ServiceType.C_SERVICE_TYPE_SALE,
                //                                             ServiceType.C_SERVICE_TYPE_RENTAL,
                //                                             MiscType.C_SALE_INSTALL_TYPE,
                //                                             MiscType.C_RENTAL_INSTALL_TYPE,
                //                                             ChangeReasonType.C_CHANGE_REASON_TYPE_CUSTOMER,
                //                                             ChangeReasonType.C_CHANGE_REASON_TYPE_SECOM,
                //                                             MiscType.C_CUSTOMER_REASON,
                //                                             MiscType.C_SECOM_REASON,
                //                                             ContractProjectCode, MaintenanceNo, null);

                List<dtInsHistory> lst = hand.GetSlipNoHistory(SlipNo);

                //Comment by Jutarat A. on 08052013 (Sort from sp_IS_GetSlipNoHistory)
                //var sortedList = from p in lst
                //                 orderby p.SlipNo
                //                 select p;
                //lst = sortedList.ToList<dtInsHistory>();
                //End Comment

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<dtInsHistory>(lst, "SlipNo", "SlipNo");
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Getting installation slip history
        /// </summary>
        /// <param name="ContractProjectCode"></param>
        /// <param name="MaintenanceNo"></param>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        public ActionResult CMS180_GetInstallationSlipHistory(string ContractProjectCode, string MaintenanceNo, string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInstallationHandler hand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<dtInstallationHistoryForView> lst = hand.GetTbt_InstallationHistoryForView(
                                                            ServiceType.C_SERVICE_TYPE_SALE,
                                                            ServiceType.C_SERVICE_TYPE_RENTAL,
                                                            MiscType.C_SALE_INSTALL_TYPE,
                                                            MiscType.C_RENTAL_INSTALL_TYPE,
                                                            ChangeReasonType.C_CHANGE_REASON_TYPE_CUSTOMER,
                                                            ChangeReasonType.C_CHANGE_REASON_TYPE_SECOM,
                                                            MiscType.C_CUSTOMER_REASON,
                                                            MiscType.C_SECOM_REASON,
                                                            ContractProjectCode, MaintenanceNo, SlipNo,
                                                            SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                                            SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                if (lst.Count > 0)
                {
                    res.ResultData = lst[0];
                }
                //res.ResultData = lst;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Getting instrument detail and load to grid
        /// </summary>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        public ActionResult CMS180_InstrumentDetailGrid(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            List<dtInstallationSlipDetailsForView> lst = new List<dtInstallationSlipDetailsForView>();
            try
            {
                IInstallationHandler hand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                //lst = hand.GetTbt_InstallationSlipDetailsForView(SlipNo, null);
                lst = hand.GetTbt_InstallationSlipDetailsForView(SlipNo, null);

                //Add by Jutarat A. on 31012013
                foreach(dtInstallationSlipDetailsForView data in lst)
                {
                    data.AddInstalledQty = (data.TotalStockOutQty?? 0) + (data.AddInstalledQty?? 0);
                    data.ReturnQty = (data.ReturnQty ?? 0) + (data.NotInstalledQty ?? 0);
                }
                //End Add

                var sortedList = from p in lst
                                 orderby p.InstrumentCode
                                 select p;

                lst = sortedList.ToList<dtInstallationSlipDetailsForView>();
                res.ResultData = CommonUtil.ConvertToXml<dtInstallationSlipDetailsForView>(lst, "Common\\CMS180_InstrumentDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Getting sub contractor
        /// </summary>
        /// <param name="MaintenanceNo"></param>
        /// <returns></returns>
        public ActionResult CMS180_SubcontractorGrid(string MaintenanceNo)
        {
            ObjectResultData res = new ObjectResultData();
            List<dtInstallationPOManagementForView> lst = new List<dtInstallationPOManagementForView>();
            try
            {
                IInstallationHandler hand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                lst = hand.GetTbt_InstallationPOManagementForView(MaintenanceNo);
                var sortedList = from p in lst
                                 orderby p.SubcontractorCode
                                 select p;

                lst = sortedList.ToList<dtInstallationPOManagementForView>();
                res.ResultData = CommonUtil.ConvertToXml<dtInstallationPOManagementForView>(lst, "Common\\CMS180_SubContractor", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Getting installation memo for view
        /// </summary>
        /// <param name="ContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult CMS180_GetInstallationMemoForview(string ContractProjectCode, string MaintenanceNo, string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInstallationHandler hand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<dtInstallationMemoForView> lst = hand.GetTbt_InstallationMemoForView(ContractProjectCode, MaintenanceNo, SlipNo);
                if (lst.Count > 0)
                {
                    res.ResultData = lst;
                }
                //res.ResultData = lst;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Download installation list data by CSV
        /// </summary>
        /// <param name="result"></param>
        public void CMS180_DownloadAsCSV() //(List<CMS180_SearchResultGridData> result) //Modify by Jutarat A. on 25062013
        {
            
            ObjectResultData res = new ObjectResultData();
            try
            {
                //Modify by Jutarat A. on 25062013
                //IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                //doSearchInstallationCondition cond = new doSearchInstallationCondition();
                //cond.slipNoNullFlag = true;
                //List<dtInstallation> lst = new List<dtInstallation>();
                //lst = handler.GetInstallationDataListForCsvFile(cond);
                //List<CMS180_SearchResultGridData> resultGridData = CommonUtil.ClonsObjectList<dtInstallation, CMS180_SearchResultGridData>(lst);
                //if (resultGridData.Count > 0)

                CMS180_ScreenParameter sParam = GetScreenObject<CMS180_ScreenParameter>();
                if (sParam.doResultCSVData != null && sParam.doResultCSVData.Count > 0)
                {
                    string strCSVResultData = CSVReportUtil.GenerateCSVData<CMS180_SearchResultGridData>(sParam.doResultCSVData, true); //resultGridData
                    this.DownloadCSVFile("InstallationBasic.csv", strCSVResultData);
                }
                //End Modify
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }           
        }                    

        /// <summary>
        /// Getting installaiton for CSV
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS180_GetInstallationForCSV()
        {
            CMS180_ScreenParameter sParam = GetScreenObject<CMS180_ScreenParameter>();
            CommonUtil cm = new CommonUtil();
            ObjectResultData res = new ObjectResultData();
            List<dtInstallation> lst = new List<dtInstallation>();
            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                doSearchInstallationCondition cond = new doSearchInstallationCondition();
                cond.slipNoNullFlag = true;
                lst = handler.GetInstallationDataListForCsvFile(cond);

                List<CMS180_SearchResultGridData> resultGridData = CommonUtil.ClonsObjectList<dtInstallation, CMS180_SearchResultGridData>(lst);
                if (resultGridData.Count <= 0)
                {                    
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }
                else
                {
                    res.ResultData = resultGridData;
                    sParam.doResultCSVData = resultGridData; //Add by Jutarat A. on 25062013
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get data for initialize search result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize search result grid.</returns>
        public ActionResult CMS180_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS180_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Load data attach to show in grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS180_LoadGridAttachedDocList(string installationSlipNo, string maintenanceNo)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = new List<dtAttachFileForGridView>();
                if (!string.IsNullOrEmpty(maintenanceNo))
                {
                    lstAttachedName.AddRange(commonhandler.GetAttachFileForGridView(maintenanceNo));
                }
                if (!string.IsNullOrEmpty(installationSlipNo))
                {
                    lstAttachedName.AddRange(commonhandler.GetAttachFileForGridView(installationSlipNo));
                    lstAttachedName.AddRange(commonhandler.GetAttachFileForGridView(installationSlipNo + "-COMPLETE"));
                }
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Common\\CMS180_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Download attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CMS180_DownloadAttach(string attachID, string installationSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                var downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(attachID), installationSlipNo);
                var downloadFileName = commonhandler.GetTbt_AttachFile(installationSlipNo, int.Parse(attachID), null);

                string fileName = downloadFileName[0].FileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = Uri.EscapeDataString(fileName);
                }
                return File(downloadFileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}
