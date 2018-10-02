//*********************************
// Create by: Jutarat A.
// Create date: 5/Oct/2011
// Update date: 5/Oct/2011
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
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;

using System.Reflection;
using System.Data;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check system suspending and user’s permission
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ActionResult CTS290_Authority(CTS290_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //Check suspending
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_NEARLY_EXPIRED_WARRANTY_SALE_CONTRACT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                sParam = new CTS290_ScreenParameter();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS290_ScreenParameter>("CTS290", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS290")]
        public ActionResult CTS290()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ViewBag.DefaultMonth = DateTime.Now.Month;
                ViewBag.DefaultYear = DateTime.Now.Year;
                ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_SEARCHPAGE;

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial SaleWarrantyExpire grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS290_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS290", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get result data of SaleWarrantyExpire Condition when click [Search] button on ‘Specify search condition’ section
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS290_GetSearchResultData(doSearchSaleWarrantyExpireCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IMaintenanceHandler maintainHandler;
            List<dtSearchSaleWarrantyExpireResult> gridDataList = new List<dtSearchSaleWarrantyExpireResult>();
            CTS290_ValidateSaleWarrantyExpirePeriod validateWarrantyExpirePeriod;

            try
            {
                //Validate required criteria
                validateWarrantyExpirePeriod = CommonUtil.CloneObject<doSearchSaleWarrantyExpireCondition, CTS290_ValidateSaleWarrantyExpirePeriod>(cond);
                ValidatorUtil.BuildErrorMessage(res, new object[] { validateWarrantyExpirePeriod }, null, false);
                if (res.IsError)
                    return Json(res);

                //Validate expire warranty month
                DateTime dtWarrantyFrom = new DateTime(cond.ExpireWarrantyYearFrom, cond.ExpireWarrantyMonthFrom, 1);
                DateTime dtWarrantyTo = new DateTime(cond.ExpireWarrantyYearTo, cond.ExpireWarrantyMonthTo, 1);
                if (dtWarrantyFrom > dtWarrantyTo)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0062, new string[] { "Expire warranty (From)", "Expire warranty (To)" }, new string[] { "WarrantyMonthFrom", "WarrantyYearFrom", "WarrantyMonthTo", "WarrantyYearTo" });
                    return Json(res);
                }

                if (String.IsNullOrEmpty(cond.OperationOfficeCode))
                    cond.OperationOfficeCode = GetAllOperationOfficeCode_CTS290();

                if (String.IsNullOrEmpty(cond.SaleContractOfficeCode))
                    cond.SaleContractOfficeCode = GetAllContractOfficeCode_CTS290();

                maintainHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                gridDataList = maintainHandler.SearchSaleWarrantyExpireList(cond);

                List<CTS290_SearchResultGridData> resultGridData = CommonUtil.ClonsObjectList<dtSearchSaleWarrantyExpireResult, CTS290_SearchResultGridData>(gridDataList);
                if (resultGridData != null && resultGridData.Count > 0)
                {
                    resultGridData = (from t in resultGridData
                                      orderby t.SiteName, t.SaleProductName
                                      select t).ToList<CTS290_SearchResultGridData>();
                }

                CTS290_ScreenParameter sParam = GetScreenObject<CTS290_ScreenParameter>();
                if (sParam.doSearchResultGridData == null)
                    sParam.doSearchResultGridData = new List<CTS290_SearchResultGridData>();

                sParam.doSearchResultGridData = resultGridData;
                sParam.dtSearchSaleWarrantyExpireResultData = gridDataList;
                UpdateScreenObject(sParam);

                res.ResultData = CommonUtil.ConvertToXml<CTS290_SearchResultGridData>(resultGridData, "Contract\\CTS290", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //public CsvActionResult CTS290_DownloadAsCSV_Test()
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    List<CTS290_SearchResultGridData> resultGridData;
        //    DataTable dtResultData = new DataTable();

        //    try
        //    {
        //        CTS290_ScreenParameter sParam = GetScreenObject<CTS290_ScreenParameter>();
        //        if (sParam == null)
        //            sParam = new CTS290_ScreenParameter();

        //        resultGridData = sParam.doSearchResultGridData;

        //        dtResultData = CommonUtil.ConvertDoListToDataTable<CTS290_SearchResultGridData>(resultGridData);
        //        return new CsvActionResult(dtResultData) { FileDownloadName = "NearlyExpiredWarrantySaleContract.csv" };

        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return null;
        //}

        /// <summary>
        /// Download the result list as CSV file when click [Download as CSV] button
        /// </summary>
        public void CTS290_DownloadAsCSV()
        {
            //string strHeader = CommonUtil.CsvHeaderGrid("Contract\\CTS290");

            //string strData = string.Empty;
            //CTS290_ScreenParameter sParam = GetScreenObject<CTS290_ScreenParameter>();
            //if (sParam != null && sParam.doSearchResultGridData != null)
            //{
            //    string strContractCodeAndOCC = string.Empty;
            //    string strSaleWarrantyPeriod = string.Empty;
            //    string strOfficeNameAndInChargeName = string.Empty;
            //    string strSiteCodeAndOptOfficeName = string.Empty;		
            //    string strRealCustomerName = string.Empty;
            //    string strSiteName = string.Empty;
            //    string strSaleProductName = string.Empty;

            //    int iNo = 1;
            //    foreach(CTS290_SearchResultGridData data in sParam.doSearchResultGridData)
            //    {
            //        strContractCodeAndOCC = String.IsNullOrEmpty(data.ContractCodeAndOCC) ? string.Empty : String.Format("\"{0}\"", data.ContractCodeAndOCC.Replace("<br/>", ""));
            //        strSaleWarrantyPeriod = String.IsNullOrEmpty(data.SaleWarrantyPeriod) ? string.Empty : String.Format("\"{0}\"", data.SaleWarrantyPeriod.Replace("<br/>", ""));
            //        strOfficeNameAndInChargeName = String.IsNullOrEmpty(data.OfficeNameAndInChargeName) ? string.Empty : String.Format("\"{0}\"", data.OfficeNameAndInChargeName.Replace("<br/>", ""));
            //        strSiteCodeAndOptOfficeName = String.IsNullOrEmpty(data.SiteCodeAndOptOfficeName) ? string.Empty : String.Format("\"{0}\"", data.SiteCodeAndOptOfficeName.Replace("<br/>", ""));
            //        strRealCustomerName = String.IsNullOrEmpty(data.RealCustomerName) ? string.Empty : String.Format("\"{0}\"", data.RealCustomerName.Replace("<br/>", ""));
            //        strSiteName = String.IsNullOrEmpty(data.SiteName) ? string.Empty : String.Format("\"{0}\"", data.SiteName.Replace("<br/>", ""));
            //        strSaleProductName = String.IsNullOrEmpty(data.SaleProductName) ? string.Empty : String.Format("\"{0}\"", data.SaleProductName.Replace("<br/>", ""));

            //        strData = string.Format("{0}{1},{2},{3},{4},{5},{6},{7},{8}\n", strData, iNo, strContractCodeAndOCC, strSaleWarrantyPeriod, strOfficeNameAndInChargeName, strSiteCodeAndOptOfficeName, strRealCustomerName, strSiteName, strSaleProductName);

            //        iNo++;
            //    }
            //}

            //string strCSVResultData = String.Format("{0}\n{1}", strHeader, strData);

            //Response.Clear();
            //Response.AddHeader("Content-Disposition", "attachment; filename=NearlyExpiredWarrantySaleContract.csv");
            //Response.ContentType = "text/csv"; //"application/force-download";
            //Response.Charset = "windows-874";
            //Response.ContentEncoding = System.Text.Encoding.GetEncoding(874); //System.Text.Encoding.GetEncoding("UTF-8");
            //Response.Write(strCSVResultData);
            //Response.End(); 

            CTS290_ScreenParameter sParam = GetScreenObject<CTS290_ScreenParameter>();
            List<CTS290_SearchResultCSVData> csvData = CommonUtil.ClonsObjectList<dtSearchSaleWarrantyExpireResult, CTS290_SearchResultCSVData>(sParam.dtSearchSaleWarrantyExpireResultData);
            string strCSVResultData = CSVReportUtil.GenerateCSVData<CTS290_SearchResultCSVData>(csvData, false);

            this.DownloadCSVFile("NearlyExpiredWarrantySaleContract.csv", strCSVResultData);
        }

        #endregion

        #region Method

        /// <summary>
        /// Get all authority OperationOffice data
        /// </summary>
        /// <returns></returns>
        private string GetAllOperationOfficeCode_CTS290()
        {
            string strOperationOfficeCode = string.Empty;
            List<string> operationOfficeCodeList = new List<string>();

            List<OfficeDataDo> oplst = (from t in CommonUtil.dsTransData.dtOfficeData
                                        where t.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO
                                        select t).ToList<OfficeDataDo>();

            //StringBuilder sbOperationOffice = new StringBuilder("");
            foreach (OfficeDataDo off in oplst)
            {
                //sbOperationOffice.AppendFormat("\'{0}\',", off.OfficeCode);
                operationOfficeCodeList.Add(off.OfficeCode);
            }

            //if (sbOperationOffice.Length > 0)
            //    strOperationOfficeCode = sbOperationOffice.ToString().Substring(0, sbOperationOffice.Length - 1);
            strOperationOfficeCode = CommonUtil.CreateCSVString(operationOfficeCodeList);

            return strOperationOfficeCode;
        }

        /// <summary>
        /// Get all authority ContractOffice data
        /// </summary>
        /// <returns></returns>
        private string GetAllContractOfficeCode_CTS290()
        {
            string strContractOfficeCode = string.Empty;
            List<string> contractOfficeCodeList = new List<string>();

            List<OfficeDataDo> clst = (from t in CommonUtil.dsTransData.dtOfficeData
                                       where t.FunctionSale != FunctionSale.C_FUNC_SALE_NO
                                       select t).ToList<OfficeDataDo>();

            //StringBuilder sbContractOffice = new StringBuilder("");
            foreach (OfficeDataDo off in clst)
            {
                //sbContractOffice.AppendFormat("\'{0}\',", off.OfficeCode);
                contractOfficeCodeList.Add(off.OfficeCode);
            }

            //if (sbContractOffice.Length > 0)
            //    strContractOfficeCode = sbContractOffice.ToString().Substring(0, sbContractOffice.Length - 1);
            strContractOfficeCode = CommonUtil.CreateCSVString(contractOfficeCodeList);

            return strContractOfficeCode;
        }

        #endregion

    }

}
