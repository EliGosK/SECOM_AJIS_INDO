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
using System.Reflection;
using SECOM_AJIS.Presentation.Quotation.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    public partial class QuotationController : BaseController
    {
        private const string QUS050_Screen = "QUS050";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult QUS050_Authority(QUS050_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<QUS050_ScreenParameter>(QUS050_Screen, param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(QUS050_Screen)]
        public ActionResult QUS050()
        {
            return View();
        }

        #endregion
        #region Actions

        /// <summary>
        /// Import quotation data
        /// </summary>
        /// <param name="ScreenID"></param>
        /// <param name="DataList"></param>
        /// <returns></returns>
        public ActionResult QUS050_ImportData(string ScreenID, List<string> DataList)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                dsImportData importData = new dsImportData()
                {
                    dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>(),
                    dtTbt_QuotationSite = new List<tbt_QuotationSite>(),
                    dtTbt_QuotationTarget = new List<tbt_QuotationTarget>(),
                    dtTbt_QuotationBasic = new List<tbt_QuotationBasic>(),
                    dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>(),
                    dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>(),
                    dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>(),
                    dtTbt_QuotationBeatGuardDetails = new List<tbt_QuotationBeatGuardDetails>(),
                    dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>(),
                    dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>()
                };

                #region Mapping Data

                List<object> impLst = new List<object>()
                {
                    importData.dtTbt_QuotationCustomer,
                    importData.dtTbt_QuotationSite,
                    importData.dtTbt_QuotationTarget,
                    importData.dtTbt_QuotationBasic,
                    importData.dtTbt_QuotationOperationType,
                    importData.dtTbt_QuotationInstrumentDetails,
                    importData.dtTbt_QuotationFacilityDetails,
                    importData.dtTbt_QuotationBeatGuardDetails,
                    importData.dtTbt_QuotationSentryGuardDetails,
                    importData.dtTbt_QuotationMaintenanceLinkage
                };

                string filePath = CommonUtil.WebPath + SECOM_AJIS.Common.Util.ConstantValue.CommonValue.IMPORT_TEMPLATE_FILE;
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNodeList nodes = doc.SelectNodes("tables/table");

                bool isError = false;
                List<string> setFailList = new List<string>();
                int lineIdx = 0;
                int nodeIdx = 0;
                for (; nodeIdx < nodes.Count; nodeIdx++)
                {
                    if (lineIdx < DataList.Count)
                    {
                        /* --- Check Table name --- */
                        string[] tbName = DataList[lineIdx].Split(",".ToCharArray());
                        if (nodes[nodeIdx].Attributes["name"].Value != tbName[0]
                            || lineIdx + 1 >= DataList.Count)
                        {
                            isError = true;
                            break;
                        }

                        lineIdx += 1;

                        /* --- Check Column --- */
                        bool isSameCol = false;
                        string[] cols = DataList[lineIdx].Split(",".ToCharArray());
                        if (cols != null)
                        {
                            if (nodes[nodeIdx].ChildNodes.Count <= cols.Length)
                            {
                                int colIdx = 0;
                                for (; colIdx < nodes[nodeIdx].ChildNodes.Count; colIdx++)
                                {
                                    string colName = cols[colIdx] == null ? "" : cols[colIdx];
                                    string cColName = nodes[nodeIdx].ChildNodes[colIdx].Attributes["name"].Value;
                                    if (cColName == null)
                                        cColName = "";

                                    colName = colName.Trim().ToUpper();
                                    cColName = cColName.Trim().ToUpper();

                                    if (colName != cColName)
                                        break;
                                }

                                bool isColOver = false;
                                if (colIdx < cols.Length)
                                {
                                    for (int nColIdx = colIdx; nColIdx < cols.Length; nColIdx++)
                                    {
                                        if (CommonUtil.IsNullOrEmpty(cols[nColIdx]) == false)
                                        {
                                            isColOver = true;
                                            break;
                                        }
                                    }
                                }
                                if (isColOver == false
                                    && colIdx == nodes[nodeIdx].ChildNodes.Count)
                                    isSameCol = true;
                            }
                        }
                        if (isSameCol == false)
                        {
                            isError = true;
                            break;
                        }

                        /* --- Get next Table --- */
                        string nextTable = null;
                        if (nodeIdx + 1 < nodes.Count)
                        {
                            nextTable = nodes[nodeIdx + 1].Attributes["name"].Value;
                        }

                        /* --- Loop fill data to each table --- */
                        lineIdx += 1;
                        while (lineIdx < DataList.Count)
                        {
                            tbName = DataList[lineIdx].Split(",".ToCharArray());
                            if (nextTable == tbName[0])
                                break;

                            bool isEmpty = true;
                            foreach (string d in tbName)
                            {
                                if (CommonUtil.IsNullOrEmpty(d) == false)
                                {
                                    isEmpty = false;
                                    break;
                                }
                            }
                            if (isEmpty)
                            {
                                isError = true;
                                break;
                            }

                            string data = DataList[lineIdx];

                            string[] lst = new string[nodes[nodeIdx].ChildNodes.Count];
                            for (int dIdx = 0; dIdx < nodes[nodeIdx].ChildNodes.Count; dIdx++)
                            {
                                if (data.Length <= 0 && dIdx < nodes[nodeIdx].ChildNodes.Count - 1)
                                {
                                    isError = true;
                                    break;
                                }

                                int tIdx = 0;
                                int cmIdx = data.IndexOf(",");
                                int ccIdx = data.IndexOf("\"");

                                string val = string.Empty;
                                if (cmIdx < 0)
                                {
                                    val = data;
                                }
                                else if (cmIdx < ccIdx || ccIdx < 0)
                                {
                                    val = data.Substring(tIdx, cmIdx);
                                    tIdx += cmIdx + 1;
                                }
                                else
                                {
                                    int cceIdx = data.IndexOf("\"", ccIdx + 1);
                                    if (cceIdx <= 0)
                                        val = data;
                                    else
                                    {
                                        val = data.Substring(tIdx + 1, cceIdx - 1);
                                        tIdx += cceIdx + 2;
                                    }
                                }

                                lst[dIdx] = val;
                                data = data.Substring(tIdx);
                            }

                            lineIdx += 1;
                            if (isError)
                                break;
                            else
                            {
                                if (nodeIdx < impLst.Count)
                                {
                                    object obj = impLst[nodeIdx];

                                    /* --- Create Object --- */
                                    object objDo = Activator.CreateInstance(obj.GetType().GetGenericArguments()[0]);

                                    MethodInfo mf = obj.GetType().GetMethod("Add");
                                    if (mf != null)
                                        mf.Invoke(obj, new object[] { objDo });

                                    for (int colIdx = 0; colIdx < nodes[nodeIdx].ChildNodes.Count; colIdx++)
                                    {
                                        bool canSetValue = CommonUtil.SetObjectValue(objDo, nodes[nodeIdx].ChildNodes[colIdx].Attributes["name"].Value, lst[colIdx] != string.Empty ? lst[colIdx] : null);
                                        if (canSetValue == false)
                                        {
                                            string v = nodes[nodeIdx].ChildNodes[colIdx].Attributes["name"].Value;
                                            if (setFailList.IndexOf(v) < 0)
                                                setFailList.Add(v);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (isError)
                        break;
                }

                if (nodeIdx < nodes.Count)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2020);
                    return Json(res);
                }
                if (setFailList.Count > 0)
                {
                    string txt = CommonUtil.TextList(setFailList.ToArray());
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2084, new string[] { txt });
                    return Json(res);
                }

                #endregion
                #region Check Mandatory

                ValidatorUtil validator = new ValidatorUtil();
                List<object> objLst = new List<object>();
                if (ScreenID == SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
                {
                    if (importData.dtTbt_QuotationCustomer.Count == 0)
                    {
                        validator.AddErrorMessage(
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "CustomerList",
                            "CustPartTypeCode, CustCode (or CustNameEN, CustNameLC, CustTypeCode, RegionCode)");
                    }
                    else
                    {
                        int cidx = 1;
                        foreach (tbt_QuotationCustomer cust in importData.dtTbt_QuotationCustomer)
                        {
                            if (CommonUtil.IsNullOrEmpty(cust.CustPartTypeCode))
                            {
                                validator.AddErrorMessage(
                                    MessageUtil.MODULE_COMMON,
                                    MessageUtil.MessageList.MSG0007,
                                    "CustPartTypeCode" + cidx,
                                    "CustPartTypeCode" + cidx);
                            }
                            if (CommonUtil.IsNullOrEmpty(cust.CustCode))
                            {
                                List<string> eLst = new List<string>();

                                if (CommonUtil.IsNullOrEmpty(cust.CustNameEN))
                                    eLst.Add("CustNameEN" + cidx);
                                if (CommonUtil.IsNullOrEmpty(cust.CustNameLC))
                                    eLst.Add("CustNameLC" + cidx);
                                if (CommonUtil.IsNullOrEmpty(cust.CustTypeCode))
                                    eLst.Add("CustTypeCode" + cidx);
                                if (CommonUtil.IsNullOrEmpty(cust.RegionCode))
                                    eLst.Add("RegionCode" + cidx);

                                if (eLst.Count == 4)
                                {
                                    validator.AddErrorMessage(
                                    MessageUtil.MODULE_COMMON,
                                    MessageUtil.MessageList.MSG0007,
                                    "Customer" + cidx,
                                    string.Format("CustCode{0} (or CustNameEN{0}, CustNameLC{0}, CustTypeCode{0}, RegionCode{0})", cidx));
                                }
                                else
                                {
                                    foreach (string s in eLst)
                                    {
                                        validator.AddErrorMessage(
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            s,
                                            s);
                                    }
                                }
                            }

                            cidx++;
                        }
                    }

                    tbt_QuotationSite site = new tbt_QuotationSite();
                    if (importData.dtTbt_QuotationSite.Count > 0)
                        site = importData.dtTbt_QuotationSite[0];
                    if (CommonUtil.IsNullOrEmpty(site.SiteNo))
                    {
                        List<string> eLst = new List<string>();
                        if (CommonUtil.IsNullOrEmpty(site.SiteNameEN))
                            eLst.Add("SiteNameEN");
                        if (CommonUtil.IsNullOrEmpty(site.SiteNameLC))
                            eLst.Add("SiteNameLC");
                        if (CommonUtil.IsNullOrEmpty(site.AddressEN))
                            eLst.Add("AddressEN");
                        if (CommonUtil.IsNullOrEmpty(site.AddressLC))
                            eLst.Add("AddressLC");
                        //if (CommonUtil.IsNullOrEmpty(site.RoadEN))
                        //    eLst.Add("RoadEN");
                        //if (CommonUtil.IsNullOrEmpty(site.RoadLC))
                        //    eLst.Add("RoadLC");
                        if (CommonUtil.IsNullOrEmpty(site.SubDistrictEN))
                            eLst.Add("SubDistrictEN");
                        if (CommonUtil.IsNullOrEmpty(site.SubDistrictLC))
                            eLst.Add("SubDistrictLC");
                        if (CommonUtil.IsNullOrEmpty(site.BuildingUsageCode))
                            eLst.Add("BuildingUsageCode");
                        if (CommonUtil.IsNullOrEmpty(site.ProvinceCode))
                            eLst.Add("ProvinceCode");
                        if (CommonUtil.IsNullOrEmpty(site.ProvinceCode))
                            eLst.Add("DistrictCode");

                        if (eLst.Count == 11)
                        {
                            validator.AddErrorMessage(
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "Site",
                            "SiteNo (or SiteNameEN, SiteNameLC, AddressEN, AddressLC, SubDistrictEN, SubDistrictLC, BuildingUsageCode, ProvinceCode, DistrictCode)");
                        }
                        else
                        {
                            foreach (string s in eLst)
                            {
                                validator.AddErrorMessage(
                                    MessageUtil.MODULE_COMMON,
                                    MessageUtil.MessageList.MSG0007,
                                    s,
                                    s);
                            }
                        }
                    }

                    if (importData.dtTbt_QuotationTarget.Count == 0)
                        importData.dtTbt_QuotationTarget.Add(new tbt_QuotationTarget());
                    foreach (tbt_QuotationTarget target in importData.dtTbt_QuotationTarget)
                    {
                        objLst.Add(CommonUtil.CloneObject<tbt_QuotationTarget, QUS050_tbt_QuotationTarget>(target));
                    }
                }
                else
                {
                    if (importData.dtTbt_QuotationTarget.Count == 0)
                        importData.dtTbt_QuotationTarget.Add(new tbt_QuotationTarget());
                    foreach (tbt_QuotationTarget target in importData.dtTbt_QuotationTarget)
                    {
                        objLst.Add(CommonUtil.CloneObject<tbt_QuotationTarget, QUS050_tbt_QuotationTarget_D>(target));
                    }

                    if (importData.dtTbt_QuotationBasic.Count == 0)
                        importData.dtTbt_QuotationBasic.Add(new tbt_QuotationBasic());
                    foreach (tbt_QuotationBasic basic in importData.dtTbt_QuotationBasic)
                    {
                        /* --- Update QuotationTargetCode --- */
                        /* ---------------------------------- */
                        basic.QuotationTargetCode = importData.dtTbt_QuotationTarget[0].QuotationTargetCode;
                        /* ---------------------------------- */

                        objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS050_tbt_QuotationBasic>(basic));
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator, objLst.ToArray());
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Business Check

                if (ScreenID == SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
                {
                    bool isFoundTarget = false;
                    bool isFoundReal = false;
                    if (importData.dtTbt_QuotationCustomer.Count > 0 && importData.dtTbt_QuotationCustomer.Count <= 2)
                    {
                        foreach (tbt_QuotationCustomer cust in importData.dtTbt_QuotationCustomer)
                        {
                            if (cust.CustPartTypeCode != SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET
                                && cust.CustPartTypeCode != SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2025);
                                return Json(res);
                            }

                            if (cust.CustPartTypeCode == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                            {
                                if (isFoundTarget == true)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2093);
                                    return Json(res);
                                }
                                else
                                    isFoundTarget = true;
                            }
                            else if (cust.CustPartTypeCode == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                            {
                                if (isFoundReal == true)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2093);
                                    return Json(res);
                                }
                                else
                                    isFoundReal = true;
                            }

                            QUS050_tbt_QuotationCustomer_BC custBC =
                                    CommonUtil.CloneObject<tbt_QuotationCustomer, QUS050_tbt_QuotationCustomer_BC>(cust);
                            ObjectResultData r = ValidatorUtil.BuildErrorMessage(custBC);
                            if (r != null)
                            {
                                if (r.IsError)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2021);
                                    return Json(res);
                                }
                            }
                        }
                        if (isFoundTarget == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2024);
                            return Json(res);
                        }
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2026);
                        return Json(res);
                    }

                    if (importData.dtTbt_QuotationSite != null)
                    {
                        if (importData.dtTbt_QuotationSite.Count > 0)
                        {
                            QUS050_tbt_QuotationSite_BC siteBC =
                                    CommonUtil.CloneObject<tbt_QuotationSite, QUS050_tbt_QuotationSite_BC>(importData.dtTbt_QuotationSite[0]);
                            ValidatorUtil.BuildErrorMessage(res, new object[] { siteBC });
                            if (res.IsError)
                                return Json(res);
                        }
                    }
                }

                #endregion
                #region Data Authority Check

                string QuotationOfficeCode = null;
                if (ScreenID == SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
                {
                    if (importData.dtTbt_QuotationTarget != null)
                    {
                        if (importData.dtTbt_QuotationTarget.Count > 0)
                            QuotationOfficeCode = importData.dtTbt_QuotationTarget[0].QuotationOfficeCode;
                    }
                }
                else
                {
                    IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        CommonUtil cmm = new CommonUtil();
                        string qt = cmm.ConvertQuotationTargetCode(importData.dtTbt_QuotationBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                        doGetQuotationDataCondition cond = new doGetQuotationDataCondition()
                        {
                            QuotationTargetCode = qt
                        };
                        List<tbt_QuotationTarget> lst = handler.GetTbt_QuotationTarget(cond);
                        if (lst.Count <= 0)
                        {
                            ISaleContractHandler shandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                            List<tbt_SaleBasic> sLst = shandler.GetTbt_SaleBasic(qt, null, true);
                            if (sLst.Count <= 0)
                            {
                                IRentralContractHandler rhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                                List<tbt_RentalContractBasic> rLst = rhandler.GetTbt_RentalContractBasic(qt, null);
                                if (rLst.Count <= 0)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2003, new string[] { importData.dtTbt_QuotationBasic[0].QuotationTargetCode });
                                    return Json(res);
                                }
                                else
                                {
                                    if (rLst[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                        QuotationOfficeCode = rLst[0].ContractOfficeCode;
                                    else
                                        QuotationOfficeCode = rLst[0].OperationOfficeCode;
                                }
                            }
                            else
                            {
                                if (sLst[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                    QuotationOfficeCode = sLst[0].ContractOfficeCode;
                                else
                                    QuotationOfficeCode = sLst[0].OperationOfficeCode;
                            }
                        }
                        else
                            QuotationOfficeCode = lst[0].OperationOfficeCode;
                    }
                }

                if (QuotationOfficeCode != null && CommonUtil.dsTransData.dtOfficeData != null)
                {
                    bool isFound = false;
                    foreach (OfficeDataDo office in CommonUtil.dsTransData.dtOfficeData)
                    {
                        if (office.OfficeCode == QuotationOfficeCode)
                        {
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2023);
                        return Json(res);
                    }
                }

                #endregion

                QUS050_ScreenParameter param = GetScreenObject<QUS050_ScreenParameter>();
                if (param != null)
                    param.ImportData = importData;

                res.ResultData = new object[] { importData, GetCurrentKey() };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Clear data in session
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS050_ClearSession()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                bool isclear = true;
                dsImportData import = QUS050_GetImportData();
                if (import != null)
                    isclear = false;
                if (isclear)
                    UpdateScreenObject(null);
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
        /// Get import data from session
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private dsImportData QUS050_GetImportData(string key = null)
        {
            try
            {
                QUS050_ScreenParameter param = GetScreenObject<QUS050_ScreenParameter>(key);
                if (param != null)
                    return param.ImportData;

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
