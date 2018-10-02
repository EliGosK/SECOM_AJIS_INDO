
//*********************************
// Create by: Waroon H.
// Create date: 29/Mar/2012
// Update date: 29/Mar/2012
//*********************************
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
using SECOM_AJIS.Presentation.Income.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS101_Authority(ICS101_ScreenParameter param)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            ObjectResultData res = new ObjectResultData();

            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            // Check User Permission
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_MONEY_COLLECTION_MANAGEMENT_INFO, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<ICS101_ScreenParameter>("ICS101", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS101")]
        public ActionResult ICS101()
        {

            ICS101_ScreenParameter param = GetScreenObject<ICS101_ScreenParameter>();

            List<tbm_Office> list = new List<tbm_Office>();
 
            if (param != null)
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = handler.GetTbm_Office();
                list = (from p in list where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO select p).ToList<tbm_Office>();
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);

                ViewBag.chkCollectionAreaList = list.ToArray();

            }

            return View();

        }

        /// <summary>
        /// Generate xml for initial money collection management information grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS101_InitialMoneyCollectionManagementInformationGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS101_MoneyCollectionManagementInformation", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Retrieve money collection management info information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS101_SearchData(ICS101_RegisterData data)
        {

            ICS101_ScreenParameter param = GetScreenObject<ICS101_ScreenParameter>();
            ICS101_RegisterData RegisterData = new ICS101_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<doGetMoneyCollectionManagementInfo> _doGetMoneyCollectionManagementInfoList = new List<doGetMoneyCollectionManagementInfo>();
            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                bool bolInput1 = false;
                bool bolInput2 = false;
                bool bolInput3 = false;

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ICS101_ScreenParameter sParam = GetScreenObject<ICS101_ScreenParameter>();

                if (data == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS101",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0006);

                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }
                if (data.Header == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS101",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0006);

                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }

                if (data.Header.dtpExpectedCollectDateFrom == Convert.ToDateTime("01-01-0001"))
                {
                    bolInput1 = true;
                    data.Header.dtpExpectedCollectDateFrom = null;
                }
                if (data.Header.dtpExpectedCollectDateFrom == null)
                { 
                    bolInput1 = true; 
                }
                if (data.Header.dtpExpectedCollectDateTo == Convert.ToDateTime("01-01-0001"))
                {
                    bolInput2 = true;
                    data.Header.dtpExpectedCollectDateTo = null;
                }
                if (data.Header.dtpExpectedCollectDateTo == null)
                { 
                    bolInput2 = true; 
                }
 
                string strDummyCollectionArea = string.Empty;

                if (data.Header.chklCollectionArea != null)
                {
                    strDummyCollectionArea = ",";
                    foreach (string _string in data.Header.chklCollectionArea)
                    {
                        strDummyCollectionArea = strDummyCollectionArea + _string + ",";
                    }
                }
                else
                {
                    bolInput3 = true;
                }

                if (bolInput1 && bolInput2 && bolInput3)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS101",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0006);

                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }

                _doGetMoneyCollectionManagementInfoList =
                    iincomeHandler.GetMoneyCollectionManagementInfoList(data.Header.dtpExpectedCollectDateFrom
                    , data.Header.dtpExpectedCollectDateTo
                    , strDummyCollectionArea);

                //if (_doGetMoneyCollectionManagementInfoList != null)
                //{
                //    if (_doGetMoneyCollectionManagementInfoList.Count > CommonValue.MAX_GRID_ROWS)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052
                //            , new string[] { CommonValue.MAX_GRID_ROWS.ToString("#,##0") });
                        
                //        return Json(res);
                //    }
                //}
                param.RegisterData = data;
                param.doGetMoneyCollectionManagementInfo = _doGetMoneyCollectionManagementInfoList;

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    List<ICS101_doGetMoneyCollectionManagementInfo> lst = CommonUtil.ClonsObjectList<doGetMoneyCollectionManagementInfo, ICS101_doGetMoneyCollectionManagementInfo>(_doGetMoneyCollectionManagementInfoList);
                    //res.ResultData = param; 
                    res.ResultData = CommonUtil.ConvertToXml<ICS101_doGetMoneyCollectionManagementInfo>(
                        lst,
                        "Income\\ICS101_MoneyCollectionManagementInformation", 
                        CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }
                else
                { res.ResultData = null; }

                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// validate input data confirm and delete money collection info information data into database
        /// </summary>
        /// <param name="DeleteReceiptNo">delete criteria</param>
        /// <returns></returns>
        public ActionResult ICS101_DeleteData(string DeleteReceiptNo)
        {

            ICS101_ScreenParameter param = GetScreenObject<ICS101_ScreenParameter>();
            ICS101_RegisterData RegisterData = new ICS101_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                List<tbt_MoneyCollectionInfo> _dotbt_MoneyCollectionInfo = new List<tbt_MoneyCollectionInfo>();

                _dotbt_MoneyCollectionInfo = iincomeHandler.DeleteTbt_MoneyCollectionInfo(DeleteReceiptNo);

                if (_dotbt_MoneyCollectionInfo == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS101",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0001,
                                             new string[] { "lblHeaderExpectedCollectDate" },
                                             new string[] { "dtpExpectedCollectDateFrom", "dtpExpectedCollectDateTo" });
                }
                if (_dotbt_MoneyCollectionInfo.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS101",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0001,
                                             new string[] { "lblHeaderExpectedCollectDate" },
                                             new string[] { "dtpExpectedCollectDateFrom", "dtpExpectedCollectDateTo" });
                }

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = "1"; }
                else
                { res.ResultData = "0"; }

                return Json(res);

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// CSV Gen File by Common function section Send Grid data to common
        /// </summary>
        /// <param name="data">all grid data list</param>
        /// <returns></returns>
        public ActionResult ICS101_SendGRIDData(ICS101_RegisterData data)
        {
            ICS101_ScreenParameter param = GetScreenObject<ICS101_ScreenParameter>();
            ICS101_RegisterData RegisterData = new ICS101_RegisterData();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {

                param.RegisterData = data;

                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = param; }
                else
                { res.ResultData = null; }

                return Json(res);

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// CSV Gen File by Common CallDownloadController and call download popup screen
        /// </summary>
        /// <returns></returns>
        public void ICS101_ExportCSV()
        {

            ICS101_ScreenParameter param = GetScreenObject<ICS101_ScreenParameter>();
            ICS101_RegisterData RegisterData = new ICS101_RegisterData();
 
            CommonUtil comUtil = new CommonUtil();
            IOfficeMasterHandler iOfficeMasterHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                
            List<tbm_Office> _dotbm_Office  = new List<tbm_Office>();

            // reuse param that send on Register Click
            if (param != null)
            {
                RegisterData = param.RegisterData;
            }

            StringBuilder sbDateFromToData = new StringBuilder();
            StringBuilder sbCollectionAreaHeader = new StringBuilder();
            StringBuilder sbCollectionAreaDetails = new StringBuilder();

            sbDateFromToData = CSVAddNewColumn(sbDateFromToData, "Expected Collect Date");
            sbDateFromToData = CSVAddNewColumn(sbDateFromToData, CommonUtil.TextDate(RegisterData.Header.dtpExpectedCollectDateFrom));
            sbDateFromToData = CSVAddNewColumn(sbDateFromToData, "to");
            sbDateFromToData = CSVAddNewColumn(sbDateFromToData, CommonUtil.TextDate(RegisterData.Header.dtpExpectedCollectDateTo));

            sbCollectionAreaHeader = CSVAddNewColumn(sbCollectionAreaHeader, "Collection Area");

            if (RegisterData.Header.chklCollectionArea != null)
            {
                foreach (string CollectionArea in RegisterData.Header.chklCollectionArea)
                {

                    _dotbm_Office = iOfficeMasterHandler.GetTbm_Office(CollectionArea);

                    if (_dotbm_Office != null)
                    {
                        if (_dotbm_Office.Count > 0)
                        {
                            sbCollectionAreaDetails = CSVAddNewColumn(sbCollectionAreaDetails, CollectionArea);
                            sbCollectionAreaDetails = CSVAddNewColumn(sbCollectionAreaDetails, _dotbm_Office[0].OfficeNameEN);
                            sbCollectionAreaDetails = CSVAddNewColumn(sbCollectionAreaDetails, _dotbm_Office[0].OfficeNameLC);
                            sbCollectionAreaDetails = CSVAddNewCRLF(sbCollectionAreaDetails);
                        }
                    }
                }
            }

            string strCSVResultData = string.Empty;
            strCSVResultData = String.Format("{0}{1}{2}{3}{4}{5}"
                , sbDateFromToData.ToString(), Environment.NewLine
                , sbCollectionAreaHeader.ToString(), Environment.NewLine
                , sbCollectionAreaDetails.ToString(), Environment.NewLine);

            if (RegisterData.doICS101_CSVGridData != null)
            {
                foreach (var d in RegisterData.doICS101_CSVGridData)
                {
                    try
                    {
                        d.ReceiptAmount = Convert.ToDecimal(d.ReceiptAmount).ToString("0.00");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        

            strCSVResultData = strCSVResultData + CSVReportUtil.GenerateCSVData<ICS101_CSVGridData>(RegisterData.doICS101_CSVGridData, true);
            
            strCSVResultData = String.IsNullOrEmpty(strCSVResultData) ? string.Empty : strCSVResultData.Replace("<br/>", "").Replace("<BR/>", "");

            this.DownloadCSVFile("MoneyCollectionManagementinfo.csv", strCSVResultData);

        }

        public StringBuilder CSVAddNewColumn(StringBuilder strCSV, string strADD)
        {
            strADD = String.Format("=\"\"{0}\"\"", strADD.Replace("\"", "\"\""));
            strCSV.AppendFormat("\"{0}\",", strADD);
            return strCSV;
        }

        public StringBuilder CSVAddNewCRLF(StringBuilder strCSV)
        {
            strCSV.AppendLine();
            return strCSV;
        }
    }
}
