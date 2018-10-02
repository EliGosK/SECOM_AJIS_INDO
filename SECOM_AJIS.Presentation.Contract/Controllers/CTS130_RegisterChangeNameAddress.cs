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
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using System.Reflection;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ActionResult CTS130_Authority(CTS130_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonHandler;
            IRentralContractHandler rentralContractHandler;
            ISaleContractHandler saleContractHandler;
            IInstallationHandler installHandler;

            string strServiceTypeCode = string.Empty;
            string strLastOCC;

            List<tbt_RentalContractBasic> listRentalContractBasic;
            List<tbt_SaleBasic> listSaleBasic;
            CommonUtil comUtil = new CommonUtil();
            string strContractCodeLong = string.Empty;

            try
            {
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP16, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //if (String.IsNullOrEmpty(sParam.ContractCode))
                //    sParam.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (String.IsNullOrEmpty(sParam.ContractCode) && sParam.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(sParam.CommonSearch.ContractCode) == false)
                        sParam.ContractCode = sParam.CommonSearch.ContractCode;
                }

                //Check strContractCode must exist
                if (String.IsNullOrEmpty(sParam.ContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                    return Json(res);
                }

                //Check user’s authority to view data
                bool bNoAuthority = false;
                strContractCodeLong = comUtil.ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                //Load contract basic data
                listRentalContractBasic = rentralContractHandler.GetTbt_RentalContractBasic(strContractCodeLong, null);
                if (listRentalContractBasic != null && listRentalContractBasic.Count > 0)
                {
                    if (listRentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].ContractOfficeCode; }).Count() == 0
                            && CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].OperationOfficeCode; }).Count() == 0)
                            bNoAuthority = true;
                    }
                    else if (listRentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                            || listRentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    {
                        if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].OperationOfficeCode; }).Count() == 0)
                            bNoAuthority = true;
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, new string[] { sParam.ContractCode });
                        return Json(res);
                    }

                    if (bNoAuthority == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                        return Json(res);
                    }
                }
                else
                {
                    //Load sale basic data
                    bNoAuthority = false;
                    listSaleBasic = saleContractHandler.GetTbt_SaleBasic(strContractCodeLong, null, FlagType.C_FLAG_ON);
                    if (listSaleBasic != null && listSaleBasic.Count > 0)
                    {
                        if (listSaleBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listSaleBasic[0].ContractOfficeCode; }).Count() == 0
                                && CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listSaleBasic[0].OperationOfficeCode; }).Count() == 0)
                                bNoAuthority = true;
                        }
                        else if (listSaleBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                        {
                            if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listSaleBasic[0].OperationOfficeCode; }).Count() == 0)
                                bNoAuthority = true;
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, new string[] { sParam.ContractCode });
                            return Json(res);
                        }

                        if (bNoAuthority == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                            return Json(res);
                        }
                    }
                    else
                    {
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                        return Json(res);
                    }
                }

                installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //Get rental contract data
                //Get last implemented OCC              
                strLastOCC = rentralContractHandler.GetLastImplementedOCC(strContractCodeLong);

                //If strLastOCC is null, Get last unimplemented OCC
                if (String.IsNullOrEmpty(strLastOCC) == true)
                {
                    strLastOCC = rentralContractHandler.GetLastUnimplementedOCC(strContractCodeLong);
                }
                else
                {
                    strServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                }

                //Get sale contract data 
                // If strLastOCC is null, Get OCC of sale 
                if (String.IsNullOrEmpty(strLastOCC) == true)
                {
                    strLastOCC = saleContractHandler.GetLastOCC(strContractCodeLong);

                    //In case of not exist both rental and sale contact
                    if (String.IsNullOrEmpty(strLastOCC) == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0102, new string[] { sParam.ContractCode }, null);
                        return Json(res);
                    }
                    else
                    {
                        strServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                    }
                }
                else
                {
                    strServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                }

                if (String.IsNullOrEmpty(strServiceTypeCode) == false)
                {
                    if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        //Validate entering conditions
                        //In case of sale contract, Installation is not processing
                        string strInstallationStatusCode = installHandler.GetInstallationStatus(strContractCodeLong);
                        if (strInstallationStatusCode != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3225);
                            return Json(res);
                        }
                    }
                }

                //sParam = new CTS130_ScreenParameter();
                sParam.ContractCode = strContractCodeLong;
                sParam.LastOCC = strLastOCC;
                sParam.ServiceTypeCode = strServiceTypeCode;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS130_ScreenParameter>("CTS130", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS130")]
        public ActionResult CTS130()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (String.IsNullOrEmpty(sParam.ServiceTypeCode) == false)
                {
                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        //Get contract data for show on screen
                        RetrieveRentalContractData_CTS130(sParam.ContractCode, sParam.LastOCC);

                        sParam = GetScreenObject<CTS130_ScreenParameter>();
                        Bind_CTS130_01(sParam.doRentalContractBasicInfo);
                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        //Get contract data for show on screen
                        RetrieveSaleContractData_CTS130(sParam.ContractCode, sParam.LastOCC);

                        sParam = GetScreenObject<CTS130_ScreenParameter>();
                        Bind_CTS130_02(sParam.doSaleContractBasicInfo);
                    }

                    sParam.ContractTargetPurchaserData = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomerWithGroup>(sParam.ContractTargetPurchaserDataTemp);
                    sParam.RealCustomerData = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomerWithGroup>(sParam.RealCustomerDataTemp);
                    sParam.SiteData = CommonUtil.ClonsObjectList<doSite, doSite>(sParam.SiteDataTemp);
                    UpdateScreenObject(sParam);

                    Bind_CTS130_04();
                    Bind_CTS130_05();
                    Bind_CTS130_06();
                    Bind_CTS130_07();
                }

                ViewBag.ServiceTypeCode = sParam.ServiceTypeCode;

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get ContractTarget Purchaser data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_GetContractTargetPurchaserListData()
        {
            ObjectResultData res = new ObjectResultData();
            List<dtCustomeGroupData> listCustomeGroup = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (sParam.ContractTargetPurchaserDataTemp != null && sParam.ContractTargetPurchaserDataTemp.Count > 0)
                {
                    listCustomeGroup = sParam.ContractTargetPurchaserDataTemp[0].CustomerGroupData;
                }

                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(listCustomeGroup, "Contract\\CTS130_ContractTargetPurchaser", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get RealCustomer data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_GetRealCustomerListData()
        {
            ObjectResultData res = new ObjectResultData();
            List<dtCustomeGroupData> listCustomeGroup = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (sParam.RealCustomerDataTemp != null && sParam.RealCustomerDataTemp.Count > 0)
                {
                    listCustomeGroup = sParam.RealCustomerDataTemp[0].CustomerGroupData;
                }

                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(listCustomeGroup, "Contract\\CTS130_RealCustomer", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get BillingTarget data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_GetBillingTargetListData()
        {
            ObjectResultData res = new ObjectResultData();
            IBillingMasterHandler billMasterHandler;
            List<CTS110_RemovalInstallationFeeGridData> gridDataList = new List<CTS110_RemovalInstallationFeeGridData>();
            List<CTS110_BillingClientData> dtBillingClientList = new List<CTS110_BillingClientData>();
            List<CTS110_BillingTargetData> dtBillingTargetList = new List<CTS110_BillingTargetData>();

            try
            {
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (sParam.doBillingTargetDetailData != null && sParam.doBillingTargetDetailData.Count > 0)
                {
                    int intSequence = -1;
                    foreach (doBillingTargetDetail dataTemp in sParam.doBillingTargetDetailData)
                    {
                        string strBillingClientCodeShort = string.Empty;
                        string strFullNameEN = string.Empty;
                        string strFullNameLC = string.Empty;

                        List<dtBillingClientData> billingClientList = billMasterHandler.GetBillingClient(dataTemp.BillingClientCode);
                        if (billingClientList.Count > 0)
                        {
                            CTS110_BillingClientData billingClient = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientList[0]);
                            billingClient.Sequence = (intSequence + 1).ToString();
                            billingClient.Status = "";
                            dtBillingClientList.Add(billingClient);

                            strBillingClientCodeShort = billingClient.BillingClientCodeShort;
                            strFullNameEN = billingClient.FullNameEN;
                            strFullNameLC = billingClient.FullNameLC;
                        }

                        CTS110_BillingTargetData billingTarget = CommonUtil.CloneObject<doBillingTargetDetail, CTS110_BillingTargetData>(dataTemp);
                        billingTarget.Sequence = (intSequence + 1).ToString();
                        billingTarget.Status = "";
                        dtBillingTargetList.Add(billingTarget);

                        CTS110_RemovalInstallationFeeGridData data = new CTS110_RemovalInstallationFeeGridData();
                        data.BillingOCC = dataTemp.BillingOCC;
                        data.BillingClientCode = strBillingClientCodeShort; //dataTemp.BillingClientCode;
                        data.BillingOfficeCode = dataTemp.BillingOfficeCode;
                        data.BillingOfficeName = GetBillingOfficeName_CTS110(dataTemp.BillingOfficeCode);

                        CommonUtil cmm = new CommonUtil();
                        data.BillingTargetCode = cmm.ConvertBillingTargetCode(dataTemp.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        data.BillingTargetName = string.Format("(1) {0} <br/>(2) {1}", strFullNameEN, strFullNameLC);
                        data.Sequence = (intSequence + 1).ToString();
                        data.Status = "";
                        gridDataList.Add(data);

                        intSequence++;
                    }
                }

                sParam.BillingClientData = dtBillingClientList;
                sParam.BillingTargetData = dtBillingTargetList;
                sParam.BillingClientOriginalData = CommonUtil.ClonsObjectList<CTS110_BillingClientData, CTS110_BillingClientData>(dtBillingClientList);
                sParam.BillingTargetOriginalData = CommonUtil.ClonsObjectList<CTS110_BillingTargetData, CTS110_BillingTargetData>(dtBillingTargetList);
                UpdateScreenObject(sParam);

                res.ResultData = CommonUtil.ConvertToXml<CTS110_RemovalInstallationFeeGridData>(gridDataList, "Contract\\CTS130_BillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve data of ContractTarget Purchaser when click [Retrieve] button on ‘Contract target/Purchaser’ section
        /// </summary>
        /// <param name="strCustomerCode"></param>
        /// <returns></returns>
        public ActionResult CTS130_RetrieveContractTargetPurchaserData(string strCustomerCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ICustomerMasterHandler customerHandler;
            List<doCustomerWithGroup> listCustomer = null;
            CommonUtil comUtil = new CommonUtil();

            try
            {
                //Mandatory check
                if (String.IsNullOrEmpty(strCustomerCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0067, null, new string[] { "PC_CustomerCodeSpecify" });
                    return Json(res);
                }

                //Get contract target or purchaser data
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                customerHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                listCustomer = customerHandler.GetCustomerWithGroup(comUtil.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (listCustomer == null || listCustomer.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0068, null, new string[] { "PC_CustomerCodeSpecify" });
                    return Json(res);
                }

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.ContractTargetPurchaserDataTemp = listCustomer;
                UpdateScreenObject(sParam);

                res.ResultData = listCustomer[0];
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve data of RealCustomer when click [Retrieve] button on ‘Real customer (End user)’ section
        /// </summary>
        /// <param name="strCustomerCode"></param>
        /// <returns></returns>
        public ActionResult CTS130_RetrieveRealCustomerData(string strCustomerCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ICustomerMasterHandler customerHandler;
            List<doCustomerWithGroup> listRealCustomer = null;
            CommonUtil comUtil = new CommonUtil();

            try
            {
                //Mandatory check
                if (String.IsNullOrEmpty(strCustomerCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0069, null, new string[] { "RC_CustomerCodeSpecify" });
                    return Json(res);
                }

                //Get real customer data
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                customerHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                listRealCustomer = customerHandler.GetCustomerWithGroup(comUtil.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (listRealCustomer == null || listRealCustomer.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0078, null, new string[] { "RC_CustomerCodeSpecify" });
                    return Json(res);
                }

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.RealCustomerDataTemp = listRealCustomer;
                UpdateScreenObject(sParam);

                res.ResultData = listRealCustomer[0];
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve data of Site when click [Retrieve] button on ‘Site’ section
        /// </summary>
        /// <param name="siteCond"></param>
        /// <returns></returns>
        public ActionResult CTS130_RetrieveSiteData(CTS130_SiteCondition siteCond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ISiteMasterHandler siteMasterHandler;
            List<doSite> listSite = null;
            CommonUtil comUtil = new CommonUtil();

            try
            {
                //Mandatory check
                if (String.IsNullOrEmpty(siteCond.RealCustomerCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0069, null, new string[] { "RC_CustomerCodeSpecify" });
                    return Json(res);
                }

                if (String.IsNullOrEmpty(siteCond.SiteCustCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0072);
                    return Json(res);
                }
                else if (String.IsNullOrEmpty(siteCond.SiteNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0070, null, new string[] { "ST_SpecifySiteNo" });
                    return Json(res);
                }

                //Get site data
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                string strSiteCode = String.Format("{0}-{1}", comUtil.ConvertSiteCode(siteCond.SiteCustCode, CommonUtil.CONVERT_TYPE.TO_LONG), siteCond.SiteNo);
                string strRealCustomerCode = comUtil.ConvertCustCode(siteCond.RealCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                siteMasterHandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                listSite = siteMasterHandler.GetSite(strSiteCode, strRealCustomerCode);
                if (listSite == null || listSite.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0071, null, new string[] { "ST_SpecifySiteNo" });
                    return Json(res);
                }

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.SiteDataTemp = listSite;
                UpdateScreenObject(sParam);

                res.ResultData = listSite[0];
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update data of ContractTarget Purchaser to screen
        /// </summary>
        /// <param name="customerData"></param>
        /// <returns></returns>
        public ActionResult CTS130_UpdateContractTargetPurchaserData(doCustomerWithGroup customerData)
        {
            ObjectResultData res = new ObjectResultData();
            List<doCustomerWithGroup> customerDataList = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (customerData != null)
                {
                    customerDataList = new List<doCustomerWithGroup>();
                    customerDataList.Add(customerData);
                }

                sParam.ContractTargetPurchaserDataTemp = customerDataList;
                UpdateScreenObject(sParam);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update data of RealCustomer to screen
        /// </summary>
        /// <param name="customerData"></param>
        /// <returns></returns>
        public ActionResult CTS130_UpdateRealCustomerData(doCustomerWithGroup customerData)
        {
            ObjectResultData res = new ObjectResultData();
            List<doCustomerWithGroup> customerDataList = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (customerData != null)
                {
                    customerDataList = new List<doCustomerWithGroup>();
                    customerDataList.Add(customerData);
                }

                sParam.RealCustomerDataTemp = customerDataList;
                UpdateScreenObject(sParam);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update data of Site to screen
        /// </summary>
        /// <param name="siteData"></param>
        /// <returns></returns>
        public ActionResult CTS130_UpdateSiteData(doSite siteData)
        {
            ObjectResultData res = new ObjectResultData();
            List<doSite> siteDataList = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (siteData != null)
                {
                    siteDataList = new List<doSite>();
                    siteDataList.Add(siteData);
                }

                sParam.SiteDataTemp = siteDataList;
                UpdateScreenObject(sParam);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search data of Site when click [Search site] button on ‘Site’ section
        /// </summary>
        /// <param name="siteCond"></param>
        /// <returns></returns>
        public ActionResult CTS130_SearchSiteData(CTS130_SiteCondition siteCond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                //Mandatory check
                if (String.IsNullOrEmpty(siteCond.RealCustomerCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0069, null, new string[] { "RC_CustomerCodeSpecify" });
                    return Json(res);
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of ContractTarget Purchaser from screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_GetContractTargetPurchaserData()
        {
            ObjectResultData res = new ObjectResultData();
            doCustomerWithGroup doCustomerData = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (sParam.ContractTargetPurchaserDataTemp != null && sParam.ContractTargetPurchaserDataTemp.Count > 0)
                {
                    doCustomerData = sParam.ContractTargetPurchaserDataTemp[0];
                }

                res.ResultData = doCustomerData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of RealCustomer from screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_GetRealCustomerData()
        {
            ObjectResultData res = new ObjectResultData();
            doCustomerWithGroup doCustomerData = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (sParam.RealCustomerDataTemp != null && sParam.RealCustomerDataTemp.Count > 0)
                {
                    doCustomerData = sParam.RealCustomerDataTemp[0];
                }

                res.ResultData = doCustomerData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of Site from screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_GetSiteData()
        {
            ObjectResultData res = new ObjectResultData();
            doSite doSiteData = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (sParam.SiteDataTemp != null && sParam.SiteDataTemp.Count > 0)
                {
                    doSiteData = sParam.SiteDataTemp[0];
                }

                res.ResultData = doSiteData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Copy data from ‘Contract target/purchaser’ section to ‘Real customer (End user)’ section 
        /// when click [Same as contract target/purchaser] button on ‘Real customer (End user)’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_SetSameContractTargetPurchaserData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<doCustomerWithGroup> doCustomerDataList = null;
            doCustomerWithGroup doCustomerData = null;
            doSite doSiteData = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (sParam.ContractTargetPurchaserDataTemp != null && sParam.ContractTargetPurchaserDataTemp.Count > 0)
                {
                    doCustomerDataList = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomerWithGroup>(sParam.ContractTargetPurchaserDataTemp);
                    doCustomerData = doCustomerDataList[0];

                    if (sParam.SiteDataTemp != null && sParam.SiteDataTemp.Count > 0)
                    {
                        if (sParam.SiteDataTemp[0].CustCode != null)
                        {
                            if (sParam.SiteDataTemp[0].CustCode != doCustomerData.CustCode)
                            {
                                sParam.SiteDataTemp = null;
                            }
                            else
                            {
                                doSiteData = sParam.SiteDataTemp[0];
                            }
                        }
                    }


                    sParam.RealCustomerDataTemp = doCustomerDataList;
                    UpdateScreenObject(sParam);
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0073);
                    return Json(res);
                }

                res.ResultData = new object[] { doCustomerData, doSiteData };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Copy data and show on ‘Site’ section when click [Copy] button on ‘Copy name and address information’ section
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS130_CopySiteName(CTS130_CopySiteNameCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<doCustomerWithGroup> doCustomerDataList = null;
            doSite doSiteData = new doSite();

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                if (cond.CopyType == "0")
                {
                    if (sParam.ContractTargetPurchaserDataTemp != null && sParam.ContractTargetPurchaserDataTemp.Count > 0)
                    {
                        doCustomerDataList = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomerWithGroup>(sParam.ContractTargetPurchaserDataTemp);

                        //Replace existing data
                        if (doCustomerDataList.Count > 0)
                        {
                            if (cond.BranchContractFlag == false)
                            {
                                doSiteData.SiteNameEN = doCustomerDataList[0].CustFullNameEN;
                                doSiteData.SiteNameLC = doCustomerDataList[0].CustFullNameLC;
                            }
                            else
                            {
                                doSiteData.SiteNameEN = String.Format("{0} {1}", doCustomerDataList[0].CustFullNameEN, cond.BranchNameEN);
                                doSiteData.SiteNameLC = String.Format("{0} {1}", doCustomerDataList[0].CustFullNameLC, cond.BranchNameLC);
                            }

                            doSiteData.AddressEN = doCustomerDataList[0].AddressEN;
                            doSiteData.AlleyEN = doCustomerDataList[0].AlleyEN;
                            doSiteData.RoadEN = doCustomerDataList[0].RoadEN;
                            doSiteData.SubDistrictEN = doCustomerDataList[0].SubDistrictEN;
                            doSiteData.AddressFullEN = doCustomerDataList[0].AddressFullEN;
                            doSiteData.AddressLC = doCustomerDataList[0].AddressLC;
                            doSiteData.AlleyLC = doCustomerDataList[0].AlleyLC;
                            doSiteData.RoadLC = doCustomerDataList[0].RoadLC;
                            doSiteData.SubDistrictLC = doCustomerDataList[0].SubDistrictLC;
                            doSiteData.AddressFullLC = doCustomerDataList[0].AddressFullLC;
                            doSiteData.DistrictCode = doCustomerDataList[0].DistrictCode;
                            doSiteData.ProvinceCode = doCustomerDataList[0].ProvinceCode;
                            doSiteData.ZipCode = doCustomerDataList[0].ZipCode;
                            doSiteData.PhoneNo = doCustomerDataList[0].PhoneNo;
                        }
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0073);
                        return Json(res);
                    }
                }
                else if (cond.CopyType == "1")
                {
                    if (sParam.RealCustomerDataTemp != null && sParam.RealCustomerDataTemp.Count > 0)
                    {
                        doCustomerDataList = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomerWithGroup>(sParam.RealCustomerDataTemp);

                        //Replace existing data
                        if (doCustomerDataList.Count > 0)
                        {
                            doSiteData.SiteNameEN = doCustomerDataList[0].CustFullNameEN;
                            doSiteData.SiteNameLC = doCustomerDataList[0].CustFullNameLC;
                            doSiteData.AddressEN = doCustomerDataList[0].AddressEN;
                            doSiteData.AlleyEN = doCustomerDataList[0].AlleyEN;
                            doSiteData.RoadEN = doCustomerDataList[0].RoadEN;
                            doSiteData.SubDistrictEN = doCustomerDataList[0].SubDistrictEN;
                            doSiteData.AddressFullEN = doCustomerDataList[0].AddressFullEN;
                            doSiteData.AddressLC = doCustomerDataList[0].AddressLC;
                            doSiteData.AlleyLC = doCustomerDataList[0].AlleyLC;
                            doSiteData.RoadLC = doCustomerDataList[0].RoadLC;
                            doSiteData.SubDistrictLC = doCustomerDataList[0].SubDistrictLC;
                            doSiteData.AddressFullLC = doCustomerDataList[0].AddressFullLC;
                            doSiteData.DistrictCode = doCustomerDataList[0].DistrictCode;
                            doSiteData.ProvinceCode = doCustomerDataList[0].ProvinceCode;
                            doSiteData.ZipCode = doCustomerDataList[0].ZipCode;
                            doSiteData.PhoneNo = doCustomerDataList[0].PhoneNo;
                        }
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0074);
                        return Json(res);
                    }
                }

                sParam.SiteDataTemp = new List<doSite>() { doSiteData };
                UpdateScreenObject(sParam);

                res.ResultData = doSiteData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get detail of BillingTarget data from screen
        /// </summary>
        /// <param name="strSequence"></param>
        /// <returns></returns>
        public ActionResult CTS130_GetBillingTargetDetail(string strSequence)
        {
            ObjectResultData res = new ObjectResultData();

            List<CTS110_BillingTargetData> dtBillingTargetList;
            CTS110_BillingTargetData billingTarget;
            List<CTS110_BillingClientData> dtBillingClientList;
            CTS110_BillingClientData billingClient;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                dtBillingTargetList = sParam.BillingTargetData;
                dtBillingClientList = sParam.BillingClientData;

                billingTarget = dtBillingTargetList.FindAll(delegate(CTS110_BillingTargetData s) { return s.Sequence == strSequence; })[0];
                billingClient = dtBillingClientList.FindAll(delegate(CTS110_BillingClientData s) { return s.Sequence == strSequence; })[0];

                sParam.BillingTargetDataTemp = CommonUtil.CloneObject<CTS110_BillingTargetData, CTS110_BillingTargetData>(billingTarget);
                sParam.BillingClientDataTemp = CommonUtil.CloneObject<CTS110_BillingClientData, CTS110_BillingClientData>(billingClient);
                sParam.SequenceTemp = strSequence;
                UpdateScreenObject(sParam);

                res.ResultData = new object[] { billingClient, billingTarget };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve BillingTarget or BillingClient when click [Retrieve] button on ‘Specify code’ section
        /// </summary>
        /// <param name="bIsTargetCode"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <returns></returns>
        public ActionResult CTS130_RetrieveBillingTargetData(string strBillingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IBillingInterfaceHandler billingHandler;
            IBillingMasterHandler billMasterHandler;
            List<tbt_BillingTarget> billingTargetList = null;
            CTS110_BillingTargetData billingTargetData = new CTS110_BillingTargetData();
            List<dtBillingClientData> billingClientDataList;
            dtBillingClientData billingClientData;

            try
            {
                if (String.IsNullOrEmpty(strBillingTargetCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                            ScreenID.C_SCREEN_ID_CP16,
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            new string[] { "lblBillingTargetCode" },
                                            new string[] { "BT_BillingTargetCode" });

                        return Json(res);
                    }

                    //Get billing target information
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    billingHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                    string strBillingTargetCodeLong = comUtil.ConvertBillingTargetCode(strBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                    billingTargetList = billingHandler.GetBillingTarget(strBillingTargetCodeLong);

                    if (billingTargetList == null || billingTargetList.Count < 1)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strBillingTargetCode });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    //strBillingClientCode = billingTargetList[0].BillingClientCode;
                    string strBillingClientCode = comUtil.ConvertBillingClientCode(billingTargetList[0].BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                

                //Get billing client information
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                string strBillingClientCodeLong = comUtil.ConvertBillingClientCode(strBillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                billingClientDataList = billMasterHandler.GetBillingClient(strBillingClientCodeLong);

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (billingClientDataList == null || billingClientDataList.Count < 1)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0138);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Refesh master data for billing target
                billingClientData = billingClientDataList[0];
                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.BillingClientDataTemp.Sequence = sParam.SequenceTemp;

                if (billingTargetList != null && billingTargetList.Count > 0)
                {
                    sParam.BillingTargetDataTemp = CommonUtil.CloneObject<tbt_BillingTarget, CTS110_BillingTargetData>(billingTargetList[0]);
                    sParam.BillingTargetDataTemp.Sequence = sParam.SequenceTemp;

                    billingTargetData = CommonUtil.CloneObject<CTS110_BillingTargetData, CTS110_BillingTargetData>(sParam.BillingTargetDataTemp);
                }
                UpdateScreenObject(sParam);

                res.ResultData = new object[] { billingClientData, billingTargetData };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve BillingTarget or BillingClient when click [Retrieve] button on ‘Specify code’ section
        /// </summary>
        /// <param name="bIsTargetCode"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <returns></returns>
        public ActionResult CTS130_RetrieveBillingClientData(string strBillingClientCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IBillingInterfaceHandler billingHandler;
            IBillingMasterHandler billMasterHandler;
            List<tbt_BillingTarget> billingTargetList = null;
            CTS110_BillingTargetData billingTargetData = new CTS110_BillingTargetData();
            List<dtBillingClientData> billingClientDataList;
            dtBillingClientData billingClientData;

            try
            {

                if (String.IsNullOrEmpty(strBillingClientCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP16,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblBillingClientCode" },
                                        new string[] { "BT_BillingClientCode" });

                    return Json(res);
                }

                //Get billing client information
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                string strBillingClientCodeLong = comUtil.ConvertBillingClientCode(strBillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                billingClientDataList = billMasterHandler.GetBillingClient(strBillingClientCodeLong);

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (billingClientDataList == null || billingClientDataList.Count < 1)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strBillingClientCode },
                                        new string[] { "BT_BillingClientCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Refesh master data for billing target
                billingClientData = billingClientDataList[0];
                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.BillingClientDataTemp.Sequence = sParam.SequenceTemp;

                if (billingTargetList != null && billingTargetList.Count > 0)
                {
                    sParam.BillingTargetDataTemp = CommonUtil.CloneObject<tbt_BillingTarget, CTS110_BillingTargetData>(billingTargetList[0]);
                    sParam.BillingTargetDataTemp.Sequence = sParam.SequenceTemp;

                    billingTargetData = CommonUtil.CloneObject<CTS110_BillingTargetData, CTS110_BillingTargetData>(sParam.BillingTargetDataTemp);
                }
                UpdateScreenObject(sParam);

                res.ResultData = new object[] { billingClientData, billingTargetData };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search BillingClient when click [Search billing client] button on ‘Specify code’ section
        /// </summary>
        /// <param name="billingClientData"></param>
        /// <returns></returns>
        public ActionResult CTS130_SearchBillingClient(dtBillingClientData billingClientData)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();

            CTS110_BillingTargetData billingTargetData = new CTS110_BillingTargetData();

            try
            {
                if (billingClientData.BillingClientCode != null
                    && billingClientData.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT)
                {
                    billingClientData.BillingClientCode = comUtil.ConvertBillingClientCode(billingClientData.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }

                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.BillingClientDataTemp.Sequence = sParam.SequenceTemp;

                sParam.BillingTargetDataTemp = billingTargetData;
                sParam.BillingTargetDataTemp.Sequence = sParam.SequenceTemp;
                UpdateScreenObject(sParam);

                res.ResultData = new object[] { billingClientData, billingTargetData };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Copy data and show on ‘Billing target detail’ section when click [Copy] button on ‘Copy name and address information’ section
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS130_CopyBillingTarget(CTS130_CopyBillingNameCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masHandler;
            List<doCustomerWithGroup> customerList;
            dtBillingClientData billingClientData = new dtBillingClientData();
            CommonUtil comUtil = new CommonUtil();

            try
            {
                masHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                string strPurchaserCustCode = comUtil.ConvertCustCode(cond.PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strRealCustCode = comUtil.ConvertCustCode(cond.RealCustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strSiteCode = comUtil.ConvertSiteCode(cond.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();

                if (cond.CopyType == "0")
                {
                    //Change spec
                    ////Load customer data of contract target
                    //customerList = masHandler.GetTbm_Customer(strPurchaserCustCode);
                    customerList = sParam.ContractTargetPurchaserDataTemp;

                    //Replace existing data
                    if (customerList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.AddressEN = customerList[0].AddressFullEN;
                        billingClientData.AddressLC = customerList[0].AddressFullLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        billingClientData.PhoneNo = customerList[0].PhoneNo;
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.BusinessTypeName = customerList[0].BusinessTypeName;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode;
                    }
                }
                else if (cond.CopyType == "1")
                {
                    //Change spec
                    ////Load customer data of contract target
                    //customerList = masHandler.GetTbm_Customer(strRealCustCode);
                    customerList = sParam.RealCustomerDataTemp;

                    //Replace existing data
                    if (customerList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.BranchNameEN = cond.BranchNameEN;
                        billingClientData.BranchNameLC = cond.BranchNameLC;
                        billingClientData.AddressEN = cond.BranchAddressEN;
                        billingClientData.AddressLC = cond.BranchAddressLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        billingClientData.PhoneNo = customerList[0].PhoneNo;
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.BusinessTypeName = customerList[0].BusinessTypeName;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode;
                    }
                }
                else if (cond.CopyType == "2")
                {
                    //Change spec
                    ////Load customer data of real customer
                    //customerList = masHandler.GetTbm_Customer(strRealCustCode);
                    customerList = sParam.RealCustomerDataTemp;

                    //Replace existing data
                    if (customerList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.AddressEN = customerList[0].AddressFullEN;
                        billingClientData.AddressLC = customerList[0].AddressFullLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        billingClientData.PhoneNo = customerList[0].PhoneNo;
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.BusinessTypeName = customerList[0].BusinessTypeName;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode;
                    }
                }
                else if (cond.CopyType == "3")
                {
                    //Change spec
                    ////Load customer data of real customer
                    //customerList = masHandler.GetTbm_Customer(strRealCustCode);

                    ////Load site data
                    //List<doGetTbm_Site> siteList = masHandler.GetTbm_Site(strSiteCode);

                    customerList = sParam.RealCustomerDataTemp;
                    List<doSite> siteList = sParam.SiteDataTemp;

                    //Replace existing data
                    if (customerList.Count > 0 && siteList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.BranchNameEN = siteList[0].SiteNameEN;
                        billingClientData.BranchNameLC = siteList[0].SiteNameLC;
                        billingClientData.AddressEN = siteList[0].AddressFullEN;
                        billingClientData.AddressLC = siteList[0].AddressFullLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        billingClientData.PhoneNo = siteList[0].PhoneNo;
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.BusinessTypeName = customerList[0].BusinessTypeName;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode;
                    }
                }

                //Refesh master data for billing target
                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                sParam.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.BillingClientDataTemp.Sequence = sParam.SequenceTemp;
                UpdateScreenObject(sParam);

                res.ResultData = billingClientData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of BillingClient
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_GetTempBillingClientData()
        {
            ObjectResultData res = new ObjectResultData();
            CTS110_BillingClientData billingClientData = null;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                billingClientData = sParam.BillingClientDataTemp;
                if (billingClientData == null)
                    billingClientData = new CTS110_BillingClientData();

                res.ResultData = billingClientData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear data of BillingTarget when click [Clear billing target] button on ‘Billing target detail’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_ClearBillingTarget()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.BillingClientDataTemp = null;
                sParam.BillingTargetDataTemp = null;
                UpdateScreenObject(sParam);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update detail of BillingTarget when click [Add/Update] button on ‘Billing target detail’ section
        /// </summary>
        /// <param name="strBillingOffice"></param>
        /// <returns></returns>
        public ActionResult CTS130_UpdateBillingTargetDetail(string strBillingOffice)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<CTS110_BillingClientData> billingClientDataList;
            List<CTS110_BillingTargetData> billingTargetDataList;

            CTS110_BillingClientData tempBillingClientData;
            CTS110_BillingTargetData tempBillingTargetData;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                billingClientDataList = sParam.BillingClientData;
                billingTargetDataList = sParam.BillingTargetData;
                tempBillingClientData = sParam.BillingClientDataTemp;
                tempBillingTargetData = sParam.BillingTargetDataTemp;

                //Validate require fields in Billing target detail section
                CTS110_ValidateBillingClientData validBillingClient = CommonUtil.CloneObject<CTS110_BillingClientData, CTS110_ValidateBillingClientData>(tempBillingClientData);
                CTS130_ValidateBillingTempData validBillingTemp = new CTS130_ValidateBillingTempData();
                validBillingTemp.BillingOfficeCode = strBillingOffice;

                ValidatorUtil.BuildErrorMessage(res, new object[] { validBillingClient, validBillingTemp });
                if (res.IsError)
                    return Json(res);

                //Validate business
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                foreach (CTS110_BillingTargetData data in billingTargetDataList)
                {
                    if (tempBillingClientData != null && data.Sequence != tempBillingClientData.Sequence)
                    {
                        if (tempBillingClientData.BillingClientCode == data.BillingClientCode && strBillingOffice == data.BillingOfficeCode)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3032);
                            return Json(res);
                        }
                    }
                }

                if (tempBillingTargetData == null)
                    tempBillingTargetData = new CTS110_BillingTargetData();

                tempBillingTargetData.BillingOfficeCode = strBillingOffice;
                tempBillingTargetData.Sequence = sParam.SequenceTemp;
                tempBillingTargetData.BillingClientCode = tempBillingClientData.BillingClientCode;

                foreach (CTS110_BillingClientData data in billingClientDataList)
                {
                    if (data.Sequence == tempBillingClientData.Sequence)
                    {
                        billingClientDataList.Remove(data);
                        billingClientDataList.Add(tempBillingClientData);
                        break;
                    }
                }

                foreach (CTS110_BillingTargetData data in billingTargetDataList)
                {
                    if (data.Sequence == tempBillingTargetData.Sequence)
                    {
                        tempBillingTargetData.BillingOCC = data.BillingOCC;

                        //Add by Jutarat A. on 24072012
                        if (data.BillingOfficeCode != tempBillingTargetData.BillingOfficeCode)
                            tempBillingTargetData.BillingTargetCode = null;

                        billingTargetDataList.Remove(data);
                        billingTargetDataList.Add(tempBillingTargetData);
                        break;
                    }
                }

                sParam.BillingClientData = billingClientDataList;
                sParam.BillingTargetData = billingTargetDataList;
                sParam.BillingClientDataTemp = null;
                sParam.BillingTargetDataTemp = null;
                UpdateScreenObject(sParam);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Reload detail of BillingTarget to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS130_RefreshBillingTargetDetail()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_BillingTargetData> billingTargetDataList;
            List<CTS110_BillingClientData> billingClientDataList;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                billingTargetDataList = sParam.BillingTargetData;
                billingClientDataList = sParam.BillingClientData;

                CommonUtil comUtil = new CommonUtil();

                List<CTS110_RemovalInstallationFeeGridData> gridDataList = new List<CTS110_RemovalInstallationFeeGridData>();
                foreach (CTS110_BillingTargetData dataTemp in billingTargetDataList)
                {
                    foreach (CTS110_BillingClientData dataClient in billingClientDataList)
                    {
                        if (dataTemp.Sequence == dataClient.Sequence)
                        {
                            CTS110_RemovalInstallationFeeGridData data = new CTS110_RemovalInstallationFeeGridData();
                            data.BillingOCC = dataTemp.BillingOCC;
                            data.BillingClientCode = dataClient.BillingClientCodeShort;
                            data.BillingOfficeCode = dataTemp.BillingOfficeCode;
                            data.BillingOfficeName = GetBillingOfficeName_CTS110(dataTemp.BillingOfficeCode);

                            //data.BillingTargetCode = dataTemp.BillingTargetCode;
                            data.BillingTargetCode = comUtil.ConvertBillingTargetCode(dataTemp.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                            data.BillingTargetName = string.Format("(1) {0} <br/>(2) {1}", dataClient.FullNameEN, dataClient.FullNameLC);
                            data.Sequence = dataClient.Sequence;
                            data.Status = dataClient.Status;
                            gridDataList.Add(data);
                        }
                    }
                }

                gridDataList = (from t in gridDataList
                                orderby t.Sequence
                                select t).ToList<CTS110_RemovalInstallationFeeGridData>();

                res.ResultData = CommonUtil.ConvertToXml<CTS110_RemovalInstallationFeeGridData>(gridDataList, "Contract\\CTS130_BillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doRegisterData"></param>
        /// <returns></returns>
        public ActionResult CTS130_RegisterChangeNameAddress(CTS130_RegisterChangeNameAddressData doRegisterData)
        {
            ObjectResultData res = new ObjectResultData();

            List<doCustomerWithGroup> listPurchaser = null;
            List<doCustomerWithGroup> listRealCustomer = null;
            List<doSite> listSite = null;

            doCustomer doPurchaser = new doCustomer();
            doCustomer doRealCustomer = new doCustomer();
            doSite doSiteData = new doSite();

            CTS130_ValidateNameReasonType validateNameReasonType = null;
            CTS130_ValidatePurchaserCustCode validatePurchaserCustCode = null;
            CTS130_ValidatePurchaserCustName validatePurchaserCustName = null;
            CTS130_ValidateContractTargetSignerTypeCode validateContractTargetSignerTypeCode = null;
            CTS130_ValidatePurchaserBranch validatePurchaserBranch = null;
            CTS130_ValidateRealCustomerCustCode validateRealCustomerCustCode = null;
            CTS130_ValidateRealCustomerCustName validateRealCustomerCustName = null;
            CTS130_ValidateSiteCode validateSiteCode = null;
            CTS130_ValidateSiteName validateSiteName = null;

            ICustomerMasterHandler customerHandler;
            ISiteMasterHandler siteHandler;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP16, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                listPurchaser = sParam.ContractTargetPurchaserDataTemp;
                listRealCustomer = sParam.RealCustomerDataTemp;
                listSite = sParam.SiteDataTemp;

                //CheckMandatory
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                //Check Name/address change reason dropdown list
                validateNameReasonType = CommonUtil.CloneObject<CTS130_RegisterChangeNameAddressData, CTS130_ValidateNameReasonType>(doRegisterData);

                //Check Contract target/Purchaser information
                if (listPurchaser == null || listPurchaser.Count == 0)
                {
                    listPurchaser = new List<doCustomerWithGroup>();
                    listPurchaser.Add(new doCustomerWithGroup());
                }

                //validatePurchaserCustCode = CommonUtil.CloneObject<doCustomerWithGroup, CTS130_ValidatePurchaserCustCode>(listPurchaser[0]);
                if (listPurchaser[0].CustCode == null)
                {
                    validatePurchaserCustName = CommonUtil.CloneObject<doCustomerWithGroup, CTS130_ValidatePurchaserCustName>(listPurchaser[0]);
                }

                validateContractTargetSignerTypeCode = CommonUtil.CloneObject<CTS130_RegisterChangeNameAddressData, CTS130_ValidateContractTargetSignerTypeCode>(doRegisterData); //Add by Jutarat A. on 20072012

                if (doRegisterData.BranchContractFlag == true)
                {
                    validatePurchaserBranch = CommonUtil.CloneObject<CTS130_RegisterChangeNameAddressData, CTS130_ValidatePurchaserBranch>(doRegisterData);
                }

                //Check Real customer (End user) information
                if (listRealCustomer == null || listRealCustomer.Count == 0)
                {
                    listRealCustomer = new List<doCustomerWithGroup>();
                    listRealCustomer.Add(new doCustomerWithGroup());
                }

                //validateRealCustomerCustCode = CommonUtil.CloneObject<doCustomerWithGroup, CTS130_ValidateRealCustomerCustCode>(listRealCustomer[0]);
                if (listRealCustomer[0].CustCode != null)
                {
                    validateRealCustomerCustName = CommonUtil.CloneObject<doCustomerWithGroup, CTS130_ValidateRealCustomerCustName>(listRealCustomer[0]);
                }

                //Check Site information 
                if (listSite == null || listSite.Count == 0)
                {
                    listSite = new List<doSite>();
                    listSite.Add(new doSite());
                }

                //validateSiteCode = CommonUtil.CloneObject<doSite, CTS130_ValidateSiteCode>(listSite[0]);
                if (listSite[0].CustCode != null)
                {
                    validateSiteName = CommonUtil.CloneObject<doSite, CTS130_ValidateSiteName>(listSite[0]);
                }

                ValidatorUtil.BuildErrorMessage(res, new object[] { validateNameReasonType, validatePurchaserCustCode, validatePurchaserCustName, validateContractTargetSignerTypeCode,
                                                validatePurchaserBranch, validateRealCustomerCustCode, validateRealCustomerCustName, validateSiteCode, validateSiteName},
                                                null, false);
                if (res.IsError)
                    return Json(res);


                //Validate contract target/purchaser, real customer and site data
                customerHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                siteHandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;

                if (listPurchaser != null && listPurchaser.Count > 0 && listPurchaser[0].CustCode == null)
                {
                    doPurchaser = CommonUtil.CloneObject<doCustomerWithGroup, doCustomer>(listPurchaser[0]);

                    customerHandler.ValidateCustomerData(doPurchaser);
                    if (doPurchaser != null)
                    {
                        if (doPurchaser.ValidateCustomerData == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0126);
                            return Json(res);
                        }
                    }
                }

                if (listRealCustomer != null && listRealCustomer.Count > 0 && listRealCustomer[0].CustCode == null)
                {
                    doRealCustomer = CommonUtil.CloneObject<doCustomerWithGroup, doCustomer>(listRealCustomer[0]);

                    customerHandler.ValidateCustomerData(doRealCustomer);
                    if (doRealCustomer != null)
                    {
                        if (doRealCustomer.ValidateCustomerData == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0127);
                            return Json(res);
                        }
                    }
                }

                if (listSite != null && listSite.Count > 0 && listSite[0].SiteCode == null)
                {
                    doSiteData = CommonUtil.CloneObject<doSite, doSite>(listSite[0]);

                    siteHandler.ValidateSiteData(doSiteData);
                    if (doSiteData != null)
                    {
                        if (doSiteData.ValidateSiteData == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0128);
                            return Json(res);
                        }
                    }
                }

                //ValidateBusiness
                ValidateBusiness_CTS130(res, sParam.ContractCode, sParam.ServiceTypeCode, doRegisterData, listSite);
                if (res.IsError)
                    return Json(res);

                if (doRegisterData.IsShowBillingTagetDetail == true)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3253);
                    return Json(res);
                }

                //ValidateBusinessForWarning
                if (sParam.ContractTargetPurchaserData != null && sParam.ContractTargetPurchaserData.Count > 0)
                {
                    if (String.IsNullOrEmpty(doRegisterData.BranchNameEN) == false && String.IsNullOrEmpty(doRegisterData.BranchNameLC) == false
                        && String.IsNullOrEmpty(doRegisterData.BranchAddressEN) == false && String.IsNullOrEmpty(doRegisterData.BranchAddressLC) == false
                        && String.IsNullOrEmpty(doRegisterData.ContactPoint) == false)
                    {
                        if (sParam.ContractTargetPurchaserData[0].IDNo != listPurchaser[0].IDNo)
                        {
                            if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
                            {
                                if (sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchNameEN == doRegisterData.BranchNameEN
                                    && sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchNameLC == doRegisterData.BranchNameLC
                                    && sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchAddressEN == doRegisterData.BranchAddressEN
                                    && sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchAddressLC == doRegisterData.BranchAddressLC
                                    && sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContactPoint == doRegisterData.ContactPoint)
                                {
                                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3226);
                                }
                            }

                        }
                    }

                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="doRegisterData"></param>
        /// <returns></returns>
        public ActionResult CTS130_ConfirmRegisterChangeNameAddress(CTS130_RegisterChangeNameAddressData doRegisterData)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP16, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();

                //ValidateBusiness
                ValidateBusiness_CTS130(res, sParam.ContractCode, sParam.ServiceTypeCode, doRegisterData, sParam.SiteDataTemp);
                if (res.IsError)
                    return Json(res);

                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    RegisterChangeNameAddressRental_CTS130(res, doRegisterData);
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    RegisterChangeNameAddressSale_CTS130(res, doRegisterData);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Method

        /// <summary>
        /// Retrieve data of RentalContract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strLastOCC"></param>
        private void RetrieveRentalContractData_CTS130(string strContractCode, string strLastOCC)
        {
            ObjectResultData res = new ObjectResultData();
            IRentralContractHandler rentralContractHandler;
            ICustomerMasterHandler customerMasterHandler;
            ISiteMasterHandler siteMasterHandler;
            IBillingInterfaceHandler billingInterfaceHandler;
            IBillingMasterHandler billingMasterHandler;
            IUserControlHandler userControlHandler;

            CommonUtil comUtil = new CommonUtil();

            dsRentalContractData dsRentalContract = null;
            List<doCustomerWithGroup> listCustomer = null;
            List<doCustomerWithGroup> listRealCustomer = null;
            List<doSite> listSite = null;
            List<doBillingTargetDetail> doBillingTargetDetailData = null;
            //List<dtBillingClientData> billingClientDataList = null;
            doRentalContractBasicInformation doRentalContractBasic = null;

            try
            {
                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                customerMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                siteMasterHandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                billingInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                userControlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;

                //Get rental contract data
                dsRentalContract = rentralContractHandler.GetEntireContract(strContractCode, strLastOCC);
                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                {
                    //Get contract target data from customer master
                    listCustomer = customerMasterHandler.GetCustomerWithGroup(dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode);

                    //Get real customer data from customer master
                    listRealCustomer = customerMasterHandler.GetCustomerWithGroup(dsRentalContract.dtTbt_RentalContractBasic[0].RealCustomerCustCode);

                    //Get site data from site master
                    //listSite = siteMasterHandler.GetSite(dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode, dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode);
                    listSite = siteMasterHandler.GetSite(dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode, null);
                }

                //Get billing target data from billing module
                doBillingTargetDetailData = billingInterfaceHandler.GetBillingTargetDetailByContractCode(strContractCode); 

                //Move to get data when load billing target to grid
                //if (doBillingTargetDetail != null)
                //{
                //    //Get billing client data from billing client master
                //    //Get all billing client code in list of doBillingTargetDetail and merge to doBillingTargetDetail[]
                //    List<dtBillingClientData> billingData = null;
                //    foreach (CTS051_DOBillingTargetDetailData data in doBillingTargetDetail)
                //    {
                //        billingData = billingMasterHandler.GetBillingClient(data.BillingClientCode);
                //        billingClientDataList.AddRange(billingData);
                //    }
                //}
                //

                //Get data for uctRentalContractBasicInformation
                doRentalContractBasic = userControlHandler.GetRentalContactBasicInformationData(strContractCode);

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.RentalContractData = dsRentalContract;
                sParam.ContractTargetPurchaserDataTemp = listCustomer;
                sParam.RealCustomerDataTemp = listRealCustomer;
                sParam.SiteDataTemp = listSite;
                sParam.doBillingTargetDetailData = doBillingTargetDetailData;
                //sParam.doBillingClientData = billingClientDataList;
                sParam.doRentalContractBasicInfo = doRentalContractBasic;
                UpdateScreenObject(sParam);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieve data of SaleContract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strLastOCC"></param>
        private void RetrieveSaleContractData_CTS130(string strContractCode, string strLastOCC)
        {
            ObjectResultData res = new ObjectResultData();
            ISaleContractHandler saleContractHandler;
            ICustomerMasterHandler customerMasterHandler;
            ISiteMasterHandler siteMasterHandler;
            IBillingInterfaceHandler billingInterfaceHandler;
            IBillingMasterHandler billingMasterHandler;
            IUserControlHandler userControlHandler;

            CommonUtil comUtil = new CommonUtil();

            dsSaleContractData dsSaleContract = null;
            List<doCustomerWithGroup> listCustomer = null;
            List<doCustomerWithGroup> listRealCustomer = null;
            List<doSite> listSite = null;
            List<doBillingTargetDetail> doBillingTargetDetailData = null;
            //List<dtBillingClientData> billingClientDataList = null;
            List<doSaleContractBasicInformation> doSaleContractBasic = null;

            try
            {
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                customerMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                siteMasterHandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                billingInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                userControlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;

                //Get sale contract data
                dsSaleContract = saleContractHandler.GetEntireContract(strContractCode, strLastOCC);
                if (dsSaleContract != null && dsSaleContract.dtTbt_SaleBasic != null && dsSaleContract.dtTbt_SaleBasic.Count > 0)
                {
                    //Get contract target data from customer master
                    listCustomer = customerMasterHandler.GetCustomerWithGroup(dsSaleContract.dtTbt_SaleBasic[0].PurchaserCustCode);

                    //Get real customer data from customer master
                    listRealCustomer = customerMasterHandler.GetCustomerWithGroup(dsSaleContract.dtTbt_SaleBasic[0].RealCustomerCustCode);

                    //Get site data from site master
                    //listSite = siteMasterHandler.GetSite(dsSaleContract.dtTbt_SaleBasic[0].SiteCode, dsSaleContract.dtTbt_SaleBasic[0].PurchaserCustCode);
                    listSite = siteMasterHandler.GetSite(dsSaleContract.dtTbt_SaleBasic[0].SiteCode, null);
                }

                //Get billing target data from billing module
                doBillingTargetDetailData = billingInterfaceHandler.GetBillingTargetDetailByContractCode(strContractCode);

                //Move to get data when load billing target to grid
                //if (doBillingTargetDetail != null)
                //{
                //    //Get billing client data from billing client master
                //    List<dtBillingClientData> billingData = null;
                //    foreach (CTS051_DOBillingTargetDetailData data in doBillingTargetDetail)
                //    {
                //        billingData = billingMasterHandler.GetBillingClient(data.BillingClientCode);
                //        billingClientDataList.AddRange(billingData);
                //    }
                //}
                //

                //Get data for uctSaleContractBasicInformation
                doSaleContractBasic = userControlHandler.GetSaleContractBasicInformationData(strContractCode, strLastOCC);

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                sParam.SaleContractData = dsSaleContract;
                sParam.ContractTargetPurchaserDataTemp = listCustomer;
                sParam.RealCustomerDataTemp = listRealCustomer;
                sParam.SiteDataTemp = listSite;
                //sParam.doBillingClientData = billingClientDataList;
                sParam.doBillingTargetDetailData = doBillingTargetDetailData;
                sParam.doSaleContractBasicInfo = doSaleContractBasic != null ? doSaleContractBasic[0] : null;
                UpdateScreenObject(sParam);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        /// <param name="doRentalContractBasicInfo"></param>
        public void Bind_CTS130_01(doRentalContractBasicInformation doRentalContractBasicInfo)
        {
            if (doRentalContractBasicInfo != null)
            {
                ViewBag.ContractCodeShort = doRentalContractBasicInfo.ContractCodeShort;
                ViewBag.UserCode = doRentalContractBasicInfo.UserCode;
                ViewBag.CustomerCode = doRentalContractBasicInfo.ContractTargetCustCodeShort;
                ViewBag.RealCustomerCustCode = doRentalContractBasicInfo.RealCustomerCustCodeShort;
                ViewBag.SiteCode = doRentalContractBasicInfo.SiteCodeShort;
                ViewBag.ImportantFlag = doRentalContractBasicInfo.ContractTargetCustomerImportant;
                ViewBag.CustFullNameEN = doRentalContractBasicInfo.ContractTargetNameEN;
                ViewBag.RealCustomerNameEN = doRentalContractBasicInfo.RealCustomerNameEN;
                ViewBag.SiteNameEN = doRentalContractBasicInfo.SiteNameEN;
                ViewBag.CustFullNameLC = doRentalContractBasicInfo.ContractTargetNameLC;
                ViewBag.RealCustomerNameLC = doRentalContractBasicInfo.RealCustomerNameLC;
                ViewBag.SiteNameLC = doRentalContractBasicInfo.SiteNameLC;

                //ViewBag.ContractOffice = doRentalContractBasicInfo.ContractOfficeName;
                //ViewBag.OperationOffice = doRentalContractBasicInfo.OperationOfficeName;
                ViewBag.ContractOffice = doRentalContractBasicInfo.ContractOfficeCodeName;
                ViewBag.OperationOffice = doRentalContractBasicInfo.OperationOfficeCodeName;
            }
        }

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        /// <param name="doSaleContractBasicInfo"></param>
        public void Bind_CTS130_02(doSaleContractBasicInformation doSaleContractBasicInfo)
        {
            if (doSaleContractBasicInfo != null)
            {
                ViewBag.ContractCodeShort = doSaleContractBasicInfo.ContractCodeShort;
                ViewBag.Purchaser = doSaleContractBasicInfo.PurchaserCustCodeShort;
                ViewBag.RealCustomerCustCode = doSaleContractBasicInfo.RealCustomerCustCodeShort;
                ViewBag.SiteCode = doSaleContractBasicInfo.SiteCodeShort;
                ViewBag.ImportantFlag = doSaleContractBasicInfo.PurchaserCustomerImportant;
                ViewBag.PurchaserNameEnglish = doSaleContractBasicInfo.PurchaserNameEN;
                ViewBag.RealCustomerNameEN = doSaleContractBasicInfo.RealCustomerNameEN;
                ViewBag.SiteNameEN = doSaleContractBasicInfo.SiteNameEN;
                ViewBag.PurchaserNameLocal = doSaleContractBasicInfo.PurchaserNameLC;
                ViewBag.RealCustomerNameLC = doSaleContractBasicInfo.RealCustomerNameLC;
                ViewBag.SiteNameLC = doSaleContractBasicInfo.SiteNameLC;
                ViewBag.ContractOffice = doSaleContractBasicInfo.ContractOfficeName;
                ViewBag.OperationOffice = doSaleContractBasicInfo.OperationOfficeName;
            }
        }
        
        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        public void Bind_CTS130_04()
        {
            CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
            if (sParam.ContractTargetPurchaserDataTemp != null && sParam.ContractTargetPurchaserDataTemp.Count > 0)
            {
                ViewBag.PC_CustomerCode = sParam.ContractTargetPurchaserDataTemp[0].CustCodeShort;
                ViewBag.PC_CustomerStatus = sParam.ContractTargetPurchaserDataTemp[0].CustStatusCodeName;
                ViewBag.PC_CustomerType = sParam.ContractTargetPurchaserDataTemp[0].CustTypeCodeName;
                ViewBag.PC_NameEnglish = sParam.ContractTargetPurchaserDataTemp[0].CustFullNameEN;
                ViewBag.PC_AddressEnglish = sParam.ContractTargetPurchaserDataTemp[0].AddressFullEN;
                ViewBag.PC_NameLocal = sParam.ContractTargetPurchaserDataTemp[0].CustFullNameLC;
                ViewBag.PC_AddressLocal = sParam.ContractTargetPurchaserDataTemp[0].AddressFullLC;
                ViewBag.PC_Nationality = sParam.ContractTargetPurchaserDataTemp[0].Nationality; //RegionName;
                ViewBag.PC_TelephoneNo = sParam.ContractTargetPurchaserDataTemp[0].PhoneNo;
                ViewBag.PC_BusinessType = sParam.ContractTargetPurchaserDataTemp[0].BusinessTypeName;
                ViewBag.PC_IDTaxID = sParam.ContractTargetPurchaserDataTemp[0].IDNo;
                ViewBag.PC_URL = sParam.ContractTargetPurchaserDataTemp[0].URL;
            }

            if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
            {
                if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
                {
                    ViewBag.PC_ContractSignerTypeVal = sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractTargetSignerTypeCode;
                    ViewBag.PC_BranchNameEnglish = sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchNameEN;
                    ViewBag.PC_BranchAddressEnglish = sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchAddressEN;
                    ViewBag.PC_BranchNameLocal = sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchNameLC;
                    ViewBag.PC_BranchAddressLocal = sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchAddressLC;
                }
            }
            else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
            {
                if (sParam.SaleContractData != null && sParam.SaleContractData.dtTbt_SaleBasic != null && sParam.SaleContractData.dtTbt_SaleBasic.Count > 0)
                {
                    ViewBag.PC_ContractSignerTypeVal = sParam.SaleContractData.dtTbt_SaleBasic[0].PurchaserSignerTypeCode;
                    ViewBag.PC_BranchNameEnglish = sParam.SaleContractData.dtTbt_SaleBasic[0].BranchNameEN;
                    ViewBag.PC_BranchAddressEnglish = sParam.SaleContractData.dtTbt_SaleBasic[0].BranchAddressEN;
                    ViewBag.PC_BranchNameLocal = sParam.SaleContractData.dtTbt_SaleBasic[0].BranchNameLC;
                    ViewBag.PC_BranchAddressLocal = sParam.SaleContractData.dtTbt_SaleBasic[0].BranchAddressLC;
                }
            }
        }

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        public void Bind_CTS130_05()
        {
            CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
            if (sParam.RealCustomerDataTemp != null && sParam.RealCustomerDataTemp.Count > 0)
            {
                ViewBag.RC_CustomerCode = sParam.RealCustomerDataTemp[0].CustCodeShort;
                ViewBag.RC_CustomerStatus = sParam.RealCustomerDataTemp[0].CustStatusCodeName;
                ViewBag.RC_CustomerType = sParam.RealCustomerDataTemp[0].CustTypeCodeName;
                ViewBag.RC_NameEnglish = sParam.RealCustomerDataTemp[0].CustFullNameEN;
                ViewBag.RC_AddressEnglish = sParam.RealCustomerDataTemp[0].AddressFullEN;
                ViewBag.RC_NameLocal = sParam.RealCustomerDataTemp[0].CustFullNameLC;
                ViewBag.RC_AddressLocal = sParam.RealCustomerDataTemp[0].AddressFullLC;
                ViewBag.RC_Nationality = sParam.RealCustomerDataTemp[0].Nationality; //RegionName;
                ViewBag.RC_TelephoneNo = sParam.RealCustomerDataTemp[0].PhoneNo;
                ViewBag.RC_BusinessType = sParam.RealCustomerDataTemp[0].BusinessTypeName;
                ViewBag.RC_IDTaxID = sParam.RealCustomerDataTemp[0].IDNo;
                ViewBag.RC_URL = sParam.RealCustomerDataTemp[0].URL;
            }
        }

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        public void Bind_CTS130_06()
        {
            CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
            if (sParam.RealCustomerDataTemp != null && sParam.RealCustomerDataTemp.Count > 0)
            {
                ViewBag.ST_SiteCodeFirst = sParam.RealCustomerDataTemp[0].SiteCustCodeShort;
            }

            if (sParam.SiteDataTemp != null && sParam.SiteDataTemp.Count > 0)
            {
                ViewBag.ST_SiteCode = sParam.SiteDataTemp[0].SiteCodeShort;
                ViewBag.ST_NameEnglish = sParam.SiteDataTemp[0].SiteNameEN;
                ViewBag.ST_AddressEnglish = sParam.SiteDataTemp[0].AddressFullEN;
                ViewBag.ST_NameLocal = sParam.SiteDataTemp[0].SiteNameLC;
                ViewBag.ST_AddressLocal = sParam.SiteDataTemp[0].AddressFullLC;
                ViewBag.ST_TelephoneNo = sParam.SiteDataTemp[0].PhoneNo;
                ViewBag.ST_Usage = sParam.SiteDataTemp[0].BuildingUsageName;
            }
        }

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        public void Bind_CTS130_07()
        {
            CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
            if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
            {
                if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
                {
                    ViewBag.ContactPointDetail = sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContactPoint;
                }
            }
            else
            {
                if (sParam.SaleContractData != null && sParam.SaleContractData.dtTbt_SaleBasic != null && sParam.SaleContractData.dtTbt_SaleBasic.Count > 0)
                {
                    ViewBag.ContactPointDetail = sParam.SaleContractData.dtTbt_SaleBasic[0].ContactPoint;
                }
            }
        }

        /// <summary>
        /// Validate business of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCodeLong"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <param name="doRegisterChangeNameAddress"></param>
        /// <param name="listSite"></param>
        private void ValidateBusiness_CTS130(ObjectResultData res, string strContractCodeLong, string strServiceTypeCode, CTS130_RegisterChangeNameAddressData doRegisterChangeNameAddress, List<doSite> listSite)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IRentralContractHandler rentralContractHandler;
            ISaleContractHandler saleContractHandler;
            IInstallationHandler installHandler;
            IMaintenanceHandler maintainHandler;

            List<tbt_RentalContractBasic> listRentalContractBasic;
            List<tbt_SaleBasic> listSaleBasic;
            CommonUtil comUtil = new CommonUtil();

            try
            {
                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                maintainHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;

                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                string strContractCodeShort = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    //Validate contract status must not be cancel
                    listRentalContractBasic = rentralContractHandler.GetTbt_RentalContractBasic(strContractCodeLong, null);
                    if (listRentalContractBasic != null && listRentalContractBasic.Count > 0)
                    {
                        if (listRentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                            || listRentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_END
                            || listRentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                ScreenID.C_SCREEN_ID_CP16,
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG3105,
                                                new string[] { strContractCodeShort });

                            return;
                        }

                        //Validate site of contracts must be the same
                        if (
                            (listRentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA
                                || (maintainHandler.GetMAContractCodeOf(listRentalContractBasic[0].ContractCode) != null)
                            )
                            && (listSite != null && listSite.Count > 0 && listSite[0].SiteCode != listRentalContractBasic[0].SiteCode)
                           )
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3262);
                            return;
                        }


                        //Validate at least one data must be edited
                        if (CompareListData_CTS130<doCustomerWithGroup>(sParam.ContractTargetPurchaserData, sParam.ContractTargetPurchaserDataTemp)
                            && CompareListData_CTS130<doCustomerWithGroup>(sParam.RealCustomerData, sParam.RealCustomerDataTemp)
                            && CompareListData_CTS130<doSite>(sParam.SiteData, sParam.SiteDataTemp)
                            && CompareListData_CTS130<CTS110_BillingClientData>(sParam.BillingClientOriginalData, sParam.BillingClientData)
                            && CompareListData_CTS130<CTS110_BillingTargetData>(sParam.BillingTargetOriginalData, sParam.BillingTargetData)
                            && (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null
                                && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0
                                && sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchNameEN == doRegisterChangeNameAddress.BranchNameEN
                                && sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchNameLC == doRegisterChangeNameAddress.BranchNameLC
                                && sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchAddressEN == doRegisterChangeNameAddress.BranchAddressEN
                                && sParam.RentalContractData.dtTbt_RentalContractBasic[0].BranchAddressLC == doRegisterChangeNameAddress.BranchAddressLC
                                && sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractTargetSignerTypeCode == doRegisterChangeNameAddress.ContractTargetSignerTypeCode
                                && sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContactPoint == doRegisterChangeNameAddress.ContactPoint)
                            )
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3291);
                            return;
                        }

                        //Validate data must not be updated by another user 
                        if (listRentalContractBasic != null && listRentalContractBasic.Count > 0)
                        {
                            if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
                            {
                                if (DateTime.Compare(listRentalContractBasic[0].UpdateDate.Value, sParam.RentalContractData.dtTbt_RentalContractBasic[0].UpdateDate.Value) != 0)
                                {
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                                }
                            }
                        }

                    }

                }
                else if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    //Validate contract status must not be cancel
                    listSaleBasic = saleContractHandler.GetTbt_SaleBasic(strContractCodeLong, null, null);
                    if (listSaleBasic != null && listSaleBasic.Count > 0)
                    {
                        if (listSaleBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                ScreenID.C_SCREEN_ID_CP16,
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG3105,
                                                new string[] { strContractCodeShort });

                            return;
                        }
                    }

                    //Validate Installation is not processing
                    string strInstallationStatusCode = installHandler.GetInstallationStatus(strContractCodeLong);
                    if (strInstallationStatusCode != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3225);
                        return;
                    }

                    //Validate site of contracts must be the same
                    if (listSaleBasic != null && listSaleBasic.Count > 0)
                    {

                        if ((maintainHandler.GetMAContractCodeOf(listSaleBasic[0].ContractCode) != null)
                            && (listSite != null && listSite.Count > 0 && listSite[0].SiteCode != listSaleBasic[0].SiteCode)
                            )
                        {
                            
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3262);
                            return;
                        }
                    }


                    //Validate at least one data must be edited
                    if (CompareListData_CTS130<doCustomerWithGroup>(sParam.ContractTargetPurchaserData, sParam.ContractTargetPurchaserDataTemp)
                        && CompareListData_CTS130<doCustomerWithGroup>(sParam.RealCustomerData, sParam.RealCustomerDataTemp)
                        && CompareListData_CTS130<doSite>(sParam.SiteData, sParam.SiteDataTemp)
                        && CompareListData_CTS130<CTS110_BillingClientData>(sParam.BillingClientOriginalData, sParam.BillingClientData)
                        && CompareListData_CTS130<CTS110_BillingTargetData>(sParam.BillingTargetOriginalData, sParam.BillingTargetData)
                        && (sParam.SaleContractData != null && sParam.SaleContractData.dtTbt_SaleBasic != null
                            && sParam.SaleContractData.dtTbt_SaleBasic.Count > 0
                            && sParam.SaleContractData.dtTbt_SaleBasic[0].BranchNameEN == doRegisterChangeNameAddress.BranchNameEN
                            && sParam.SaleContractData.dtTbt_SaleBasic[0].BranchNameLC == doRegisterChangeNameAddress.BranchNameLC
                            && sParam.SaleContractData.dtTbt_SaleBasic[0].BranchAddressEN == doRegisterChangeNameAddress.BranchAddressEN
                            && sParam.SaleContractData.dtTbt_SaleBasic[0].BranchAddressLC == doRegisterChangeNameAddress.BranchAddressLC
                            && sParam.SaleContractData.dtTbt_SaleBasic[0].PurchaserSignerTypeCode == doRegisterChangeNameAddress.ContractTargetSignerTypeCode
                            && sParam.SaleContractData.dtTbt_SaleBasic[0].ContactPoint == doRegisterChangeNameAddress.ContactPoint)
                        )
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3291);
                        return;
                    }

                    //Validate data must not be updated by another user 
                    if (listSaleBasic != null && listSaleBasic.Count > 0)
                    {
                        if (sParam.SaleContractData != null && sParam.SaleContractData.dtTbt_SaleBasic != null && sParam.SaleContractData.dtTbt_SaleBasic.Count > 0)
                        {
                            if (DateTime.Compare(listSaleBasic[0].UpdateDate.Value, sParam.SaleContractData.dtTbt_SaleBasic[0].UpdateDate.Value) != 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Register ChangeName and Address of Rental to database
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doRegisterData"></param>
        private void RegisterChangeNameAddressRental_CTS130(ObjectResultData res, CTS130_RegisterChangeNameAddressData doRegisterData)
        {
            List<doCustomerWithGroup> listPurchaser = null;
            List<doCustomerWithGroup> listRealCustomer = null;
            List<doSite> listSite = null;

            List<doCustomerWithGroup> tempListPurchaser = null;
            List<doCustomerWithGroup> tempListRealCustomer = null;
            List<doSite> tempListSite = null;

            List<doCustomer> doContractTargetPurchaserTempData = null;
            List<doCustomer> doRealCustomerTempData = null;

            doCustomerTarget doRegisCustomerTarget = null;
            doCustomerTarget doRegisRealCustomer = null;
            List<tbt_RentalSecurityBasic> doUnImpleTbtRentalSecurityBasic = null;
            List<tbt_RentalContractBasic> doImpleTbtRentalContractBasic = null;
            List<dtTbt_BillingTempListForView> doTbt_BillingTemp = null;

            dsRentalContractData dsRentalContract = null;
            dsRentalContractData dsRegisterRentalContract = new dsRentalContractData();
            dsRentalContractData dsRentalContractResult = null;
            dsRentalContractData dsRentalContractOriginal = null;

            List<CTS110_BillingClientData> billingClientDataList;
            List<CTS110_BillingTargetData> billingTargetDataList;
            List<CTS110_BillingClientData> billingClientOriginalDataList;
            List<CTS110_BillingTargetData> billingTargetOriginalDataList;

            ICustomerMasterHandler cusMasterHandler;
            IBillingMasterHandler billMasterHandler;
            IRentralContractHandler rentralHandler;
            ICommonContractHandler contractHandler;
            IBillingInterfaceHandler billInterfaceHandler;
            IBillingTempHandler billTempHandler;

            //Create temporary parameter 
            bool blnCSChangeFlag = false;
            //bool blnContractSignerChangeFlag = false; //Add by Jutarat A. on 18092012
            bool blnRCChangeFlag = false;
            bool blnSiteChangeFlag = false;
            string strHisOCC = String.Empty;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                listPurchaser = sParam.ContractTargetPurchaserData;
                listRealCustomer = sParam.RealCustomerData;
                listSite = sParam.SiteData;
                tempListPurchaser = sParam.ContractTargetPurchaserDataTemp;
                tempListRealCustomer = sParam.RealCustomerDataTemp;
                tempListSite = sParam.SiteDataTemp;
                billingClientDataList = sParam.BillingClientData;
                billingTargetDataList = sParam.BillingTargetData;
                billingClientOriginalDataList = sParam.BillingClientOriginalData;
                billingTargetOriginalDataList = sParam.BillingTargetOriginalData;

                dsRentalContractOriginal = sParam.RentalContractData;

                cusMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                contractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                billInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                billTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //Create data object doRegisCustomerTarget
                    doRegisCustomerTarget = new doCustomerTarget();

                    //Prepare data
                    doCustomer customerData = new doCustomer();
                    List<dtCustomerGroup> customerGroupData = new List<dtCustomerGroup>();
                    if (tempListPurchaser != null && tempListPurchaser.Count > 0)
                    {
                        customerData = CommonUtil.CloneObject<doCustomerWithGroup, doCustomer>(tempListPurchaser[0]);
                        customerGroupData = CommonUtil.ClonsObjectList<dtCustomeGroupData, dtCustomerGroup>(tempListPurchaser[0].CustomerGroupData);
                    }

                    doRegisCustomerTarget.doCustomer = customerData;
                    doRegisCustomerTarget.dtCustomerGroup = customerGroupData;
                    doRegisCustomerTarget.doSite = null;

                    //Get contract target data from customer master for comparing
                    if (tempListPurchaser != null && tempListPurchaser.Count > 0)
                    {
                        doContractTargetPurchaserTempData = cusMasterHandler.GetCustomer(tempListPurchaser[0].CustCode);
                        if (tempListPurchaser[0].CustCode == null || (doContractTargetPurchaserTempData != null && doContractTargetPurchaserTempData.Count > 0))
                        {
                            List<doCustomer> doContractTargetPurchaserModifyData = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomer>(tempListPurchaser);

                            //Manage contract target information
                            if (tempListPurchaser[0].CustCode == null || CompareObjectData_CTS130(doContractTargetPurchaserModifyData[0], doContractTargetPurchaserTempData[0]) == false)
                            {
                                //Manage data
                                doRegisCustomerTarget = cusMasterHandler.ManageCustomerTarget(doRegisCustomerTarget);

                                //Set temporary parameter 
                                ////Add by Jutarat A. on 18092012
                                //if (dsRentalContractOriginal.dtTbt_RentalContractBasic != null
                                //    && dsRentalContractOriginal.dtTbt_RentalContractBasic[0].ContractTargetSignerTypeCode != doRegisterData.ContractTargetSignerTypeCode)
                                //{
                                //    blnContractSignerChangeFlag = true;
                                //}
                                ////End Add
                                //else
                                //{
                                //    blnCSChangeFlag = true;
                                //}
                                blnCSChangeFlag = true;
                            }
                        }

                        //Update customer status
                        cusMasterHandler.ManageCustomerInformation(doRegisCustomerTarget.doCustomer.CustCode);

                        //Set temporary parameter in case contract target code or contract signer type is changed
                        if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null
                            && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            if (tempListPurchaser[0].CustCode != sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractTargetCustCode
                                || doRegisterData.ContractTargetSignerTypeCode != sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractTargetSignerTypeCode)
                            {
                                //Set temporary parameter 
                                ////Add by Jutarat A. on 18092012
                                //if (dsRentalContractOriginal.dtTbt_RentalContractBasic != null
                                //    && dsRentalContractOriginal.dtTbt_RentalContractBasic[0].ContractTargetSignerTypeCode != doRegisterData.ContractTargetSignerTypeCode)
                                //{
                                //    blnContractSignerChangeFlag = true;
                                //}
                                ////End Add
                                //else
                                //{
                                //    blnCSChangeFlag = true;
                                //}
                                blnCSChangeFlag = true;
                            }
                        }
                    }


                    //Create data object doRegisCustomerTarget
                    doRegisRealCustomer = new doCustomerTarget();

                    //Prepare data
                    customerData = new doCustomer();
                    customerGroupData = new List<dtCustomerGroup>();
                    if (tempListRealCustomer != null && tempListRealCustomer.Count > 0)
                    {
                        customerData = CommonUtil.CloneObject<doCustomerWithGroup, doCustomer>(tempListRealCustomer[0]);
                        customerGroupData = CommonUtil.ClonsObjectList<dtCustomeGroupData, dtCustomerGroup>(tempListRealCustomer[0].CustomerGroupData);
                    }

                    doSite siteData = new doSite();
                    if (sParam.SiteDataTemp != null && sParam.SiteDataTemp.Count > 0)
                    {
                        siteData = CommonUtil.CloneObject<doSite, doSite>(sParam.SiteDataTemp[0]);
                    }

                    doRegisRealCustomer.doCustomer = customerData;
                    doRegisRealCustomer.dtCustomerGroup = customerGroupData;
                    doRegisRealCustomer.doSite = siteData;

                    //Get real customer data from customer master for comparing
                    if (tempListRealCustomer != null && tempListRealCustomer.Count > 0)
                    {
                        doRealCustomerTempData = cusMasterHandler.GetCustomer(tempListRealCustomer[0].CustCode);
                        if (tempListRealCustomer[0].CustCode == null || (doRealCustomerTempData != null && doRealCustomerTempData.Count > 0))
                        {
                            List<doCustomer> doRealCustomerModifyData = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomer>(tempListRealCustomer);

                            //Manage real customer information
                            if (tempListRealCustomer[0].CustCode == null || CompareObjectData_CTS130(doRealCustomerModifyData[0], doRealCustomerTempData[0]) == false)
                            {
                                //Manage data
                                doRegisRealCustomer = cusMasterHandler.ManageCustomerTarget(doRegisRealCustomer);

                                //Set temporary parameter 
                                blnRCChangeFlag = true;
                            }
                            else if (tempListSite[0].SiteCode == null)
                            {
                                //Manage data
                                doRegisRealCustomer = cusMasterHandler.ManageCustomerTarget(doRegisRealCustomer);

                                //Set temporary parameter 
                                blnSiteChangeFlag = true;
                            }
                        }

                        //Update customer status
                        cusMasterHandler.ManageCustomerInformation(doRegisRealCustomer.doCustomer.CustCode);

                        //Set temporary parameter in case real customer code is changed
                        if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null
                            && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            if (tempListRealCustomer[0].CustCode != sParam.RentalContractData.dtTbt_RentalContractBasic[0].RealCustomerCustCode)
                            {
                                //Set temporary parameter 
                                blnRCChangeFlag = true;
                            }
                        }

                        //Set temporary parameter in site code is changed
                        if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null
                            && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            if (tempListSite[0].SiteCode != sParam.RentalContractData.dtTbt_RentalContractBasic[0].SiteCode)
                            {
                                //Set temporary parameter 
                                blnSiteChangeFlag = true;
                            }
                        }
                    }


                    //Create data object
                    tbm_BillingClient doRegisBillingClient = new tbm_BillingClient();
                    List<doBillingTempBasic> doRegisBillingBasicList = new List<doBillingTempBasic>();

                    //Get data from billing temp for updating in case billing target exist in Billing temp
                    doTbt_BillingTemp = contractHandler.GetTbt_BillingTempListForView(sParam.ContractCode, null);

                    /*--- Manage billing target in list ---*/
                    //Manage billing client
                    if (CompareListData_CTS130<CTS110_BillingClientData>(billingClientOriginalDataList, billingClientDataList) == false) //Add by Jutarat A. on 24072012
                    {
                        foreach (CTS110_BillingClientData billClient in billingClientDataList)
                        {
                            if (String.IsNullOrEmpty(billClient.BillingClientCode))
                            {
                                doRegisBillingClient = CommonUtil.CloneObject<CTS110_BillingClientData, tbm_BillingClient>(billClient);
                                billClient.BillingClientCode = billMasterHandler.ManageBillingClient(doRegisBillingClient);

                                foreach (CTS110_BillingTargetData target in billingTargetDataList)
                                {
                                    if (billClient.Sequence == target.Sequence)
                                    {
                                        target.BillingClientCode = billClient.BillingClientCode;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //Modify by Jutarat A. on 24072012
                    ////Manage billing target
                    //doBillingTempBasic doRegisBillingBasic;
                    //foreach (CTS110_BillingTargetData orgData in billingTargetOriginalDataList)
                    //{
                    //    foreach (CTS110_BillingTargetData newData in billingTargetDataList)
                    //    {
                    //        if (orgData.Sequence == newData.Sequence)
                    //        {
                    //            CTS110_BillingClientData billClient = new CTS110_BillingClientData();
                    //            List<CTS110_BillingClientData> billClientList = billingClientDataList.FindAll(delegate(CTS110_BillingClientData data) { return data.Sequence == newData.Sequence; });
                    //            if (billClientList != null && billClientList.Count > 0)
                    //                billClient = billClientList[0];

                    //            doRegisBillingBasic = new doBillingTempBasic();
                    //            if (orgData.BillingClientCode != billClient.BillingClientCode //newData.BillingClientCode
                    //                || orgData.BillingOfficeCode != newData.BillingOfficeCode
                    //                || orgData.BillingTargetCode != newData.BillingTargetCode)
                    //            {
                    //                //Prepare data
                    //                doRegisBillingBasic.ContractCode = sParam.ContractCode;
                    //                doRegisBillingBasic.BillingOCC = newData.BillingOCC;
                    //                doRegisBillingBasic.BillingClientCode = billClient.BillingClientCode; //newData.BillingClientCode;
                    //                doRegisBillingBasic.BillingOfficeCode = newData.BillingOfficeCode;
                    //                doRegisBillingBasic.BillingTargetCode = newData.BillingTargetCode;

                    //                doRegisBillingBasicList.Add(doRegisBillingBasic);
                    //            }
                    //        }
                    //    }
                    //}
                    if (CompareListData_CTS130<CTS110_BillingTargetData>(billingTargetOriginalDataList, billingTargetDataList) == false)
                    {
                        doBillingTempBasic doRegisBillingBasic;
                        foreach (CTS110_BillingTargetData newData in billingTargetDataList)
                        {
                            //Prepare data
                            doRegisBillingBasic = new doBillingTempBasic();
                            doRegisBillingBasic.ContractCode = sParam.ContractCode;
                            doRegisBillingBasic.BillingOCC = newData.BillingOCC;
                            doRegisBillingBasic.BillingClientCode = newData.BillingClientCode;
                            doRegisBillingBasic.BillingOfficeCode = newData.BillingOfficeCode;
                            doRegisBillingBasic.BillingTargetCode = newData.BillingTargetCode;

                            doRegisBillingBasicList.Add(doRegisBillingBasic);
                        }
                    }
                    //End Modify

                    //Send data to billing module
                    if (doRegisBillingBasicList != null && doRegisBillingBasicList.Count > 0)
                    {
                        billInterfaceHandler.SendBilling_ChangeName(doRegisBillingBasicList);
                    }
                    /*------------------------------------*/


                    //Check billing OCC exist in billing temp
                    foreach (dtTbt_BillingTempListForView tempData in doTbt_BillingTemp)
                    {
                        //foreach (CTS110_BillingTargetData targetData in billingTargetDataList)
                        foreach (doBillingTempBasic targetData in doRegisBillingBasicList) //Modify by Jutarat A. on 24072012
                        {
                            if (tempData.BillingOCC == targetData.BillingOCC)
                            {
                                if (tempData.SendFlag == BillingTemp.C_BILLINGTEMP_FLAG_KEEP)
                                {
                                    //Update data in Billing temp
                                    billTempHandler.UpdateBillingTempByBillingTarget(sParam.ContractCode,
                                                                                        tempData.BillingClientCode,
                                                                                        tempData.BillingOfficeCode,
                                                                                        tempData.BillingTargetCode,
                                                                                        targetData.BillingClientCode,
                                                                                        targetData.BillingOfficeCode,
                                                                                        targetData.BillingTargetCode);
                                }
                            }
                        }
                    }

                    /*--- Update rental security basic (Unimplement) ---*/
                    //Check unimplemented OCC
                    string strLastUnimOCC = rentralHandler.GetLastUnimplementedOCC(sParam.ContractCode);
                    if (String.IsNullOrEmpty(strLastUnimOCC) == false)
                    {
                        //Get last umimplement data
                        doUnImpleTbtRentalSecurityBasic = rentralHandler.GetTbt_RentalSecurityBasic(sParam.ContractCode, strLastUnimOCC);

                        //Prepare data before saving		 
                        if (doUnImpleTbtRentalSecurityBasic != null && doUnImpleTbtRentalSecurityBasic.Count > 0)
                        {
                            doUnImpleTbtRentalSecurityBasic[0].ChangeNameReasonType = doRegisterData.ChangeNameReasonType;

                            //Update rental security basic (Unimplement data)
                            doUnImpleTbtRentalSecurityBasic = rentralHandler.UpdateTbt_RentalSecurityBasic(doUnImpleTbtRentalSecurityBasic[0]);
                        }
                    }
                    /*----------------------------------------*/

                    /*--- Update rental security basic (Implement) ---*/
                    //Check implemented OCC
                    string strLastImOCC = rentralHandler.GetLastImplementedOCC(sParam.ContractCode);
                    if (String.IsNullOrEmpty(strLastImOCC) == false)
                    {
                        //Get last implement data
                        dsRentalContract = rentralHandler.GetEntireContract(sParam.ContractCode, strLastImOCC);
                        if (dsRentalContract != null)
                        {
                            //Check data updated by other person 
                            if (dsRentalContractOriginal != null)
                            {
                                if ((dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                                    && (dsRentalContractOriginal.dtTbt_RentalContractBasic != null && dsRentalContractOriginal.dtTbt_RentalContractBasic.Count > 0))
                                {
                                    if (DateTime.Compare(dsRentalContract.dtTbt_RentalContractBasic[0].UpdateDate.Value, dsRentalContractOriginal.dtTbt_RentalContractBasic[0].UpdateDate.Value) != 0)
                                    {
                                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                                    }
                                }
                            }

                            //Generate security occurrence
                            string strNewImOCC = rentralHandler.GenerateContractOCC(sParam.ContractCode, true);

                            //Set data to dsRegisterRentalContract.doTbt_RentalContractBasic 
                            if (dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();

                                tbt_RentalContractBasic rentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(dsRentalContract.dtTbt_RentalContractBasic[0]);
                                rentalContractBasic.ContractCode = sParam.ContractCode;
                                rentalContractBasic.LastOCC = strNewImOCC;

                                //rentalContractBasic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME;
                                if (rentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                                    rentalContractBasic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP;
                                else
                                    rentalContractBasic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME;

                                rentalContractBasic.ContractTargetCustCode = doRegisCustomerTarget.doCustomer.CustCode;
                                rentalContractBasic.ContractTargetSignerTypeCode = doRegisterData.ContractTargetSignerTypeCode;
                                rentalContractBasic.RealCustomerCustCode = doRegisRealCustomer.doCustomer.CustCode;
                                rentalContractBasic.BranchNameEN = doRegisterData.BranchNameEN;
                                rentalContractBasic.BranchNameLC = doRegisterData.BranchNameLC;
                                rentalContractBasic.BranchAddressEN = doRegisterData.BranchAddressEN;
                                rentalContractBasic.BranchAddressLC = doRegisterData.BranchAddressLC;
                                rentalContractBasic.ContactPoint = doRegisterData.ContactPoint;
                                rentalContractBasic.SiteCode = doRegisRealCustomer.doSite.SiteCode;
                                rentalContractBasic.LastChangeImplementDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                rentalContractBasic.ChangeNameProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                //rentalContractBasic.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                //rentalContractBasic.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                //rentalContractBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                //rentalContractBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                dsRegisterRentalContract.dtTbt_RentalContractBasic.Add(rentalContractBasic);
                            }

                            //Set data to dsRegisterRentalContract.dtTbt_RentalSecurityBasic 
                            if (dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();

                                tbt_RentalSecurityBasic rentalSecurityBasic = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(dsRentalContract.dtTbt_RentalSecurityBasic[0]);
                                rentalSecurityBasic.ContractCode = sParam.ContractCode;
                                rentalSecurityBasic.OCC = strNewImOCC;

                                if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                                    rentalSecurityBasic.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP;
                                else
                                    rentalSecurityBasic.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME;

                                //Add by Jutarat A. 04102012
                                rentalSecurityBasic.QuotationTargetCode = null;
                                rentalSecurityBasic.QuotationAlphabet = null;
                                rentalSecurityBasic.SalesmanEmpNo1 = null;
                                rentalSecurityBasic.SalesmanEmpNo2 = null;
                                rentalSecurityBasic.SalesSupporterEmpNo = null;
                                //End Add

                                rentalSecurityBasic.ChangeImplementDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                //rentalSecurityBasic.NormalContractFee = null;
                                //rentalSecurityBasic.OrderContractFee = null;
                                //rentalSecurityBasic.OrderContractFeePayMethod = null;
                                //rentalSecurityBasic.ContractFeeOnStop = null;
                                rentalSecurityBasic.NormalAdditionalDepositFee = null;
                                rentalSecurityBasic.OrderAdditionalDepositFee = null;
                                rentalSecurityBasic.DepositFeeBillingTiming = null;
                                rentalSecurityBasic.PlanCode = null; //Add by Jutarat A. 04102012
                                rentalSecurityBasic.ApproveNo1 = null;
                                rentalSecurityBasic.ApproveNo2 = null;
                                rentalSecurityBasic.ApproveNo3 = null;
                                rentalSecurityBasic.ApproveNo4 = null;
                                rentalSecurityBasic.ApproveNo5 = null;
                                rentalSecurityBasic.AlmightyProgramEmpNo = "-";
                                rentalSecurityBasic.CounterNo = 0;
                                rentalSecurityBasic.ChangeReasonType = null;
                                rentalSecurityBasic.ChangeNameReasonType = doRegisterData.ChangeNameReasonType;
                                rentalSecurityBasic.StopCancelReasonType = null;
                                rentalSecurityBasic.UninstallType = null;
                                rentalSecurityBasic.ContractDocPrintFlag = null;
                                rentalSecurityBasic.ExpectedInstallationCompleteDate = null;
                                //rentalSecurityBasic.InstallationCompleteFlag = FlagType.C_FLAG_OFF; 
                                rentalSecurityBasic.InstallationCompleteFlag = null;
                                rentalSecurityBasic.InstallationCompleteEmpNo = null;
                                rentalSecurityBasic.InstallationSlipNo = null;
                                rentalSecurityBasic.InstallationCompleteDate = null;
                                rentalSecurityBasic.InstallationTypeCode = null;
                                rentalSecurityBasic.NegotiationStaffEmpNo1 = null;
                                rentalSecurityBasic.NegotiationStaffEmpNo2 = null;
                                rentalSecurityBasic.CalIndex = null;
                                //rentalSecurityBasic.FacilityPassYear = null;
                                //rentalSecurityBasic.FacilityPassMonth = null;
                                //rentalSecurityBasic.InsuranceCoverageAmount = null;
                                //rentalSecurityBasic.InsuranceTypeCode = null;
                                //rentalSecurityBasic.MonthlyInsuranceFee = null;
                                rentalSecurityBasic.NormalInstallFee = null;
                                rentalSecurityBasic.OrderInstallFee = null;
                                rentalSecurityBasic.OrderInstallFee_ApproveContract = null;
                                rentalSecurityBasic.OrderInstallFee_CompleteInstall = null;
                                rentalSecurityBasic.OrderInstallFee_StartService = null;
                                rentalSecurityBasic.InstallFeePaidBySECOM = null;
                                rentalSecurityBasic.InstallFeeRevenueBySECOM = null;
                                rentalSecurityBasic.DispatchTypeCode = null;
                                //rentalSecurityBasic.AdditionalFee1 = null;
                                //rentalSecurityBasic.AdditionalFee2 = null;
                                //rentalSecurityBasic.AdditionalFee3 = null;
                                //rentalSecurityBasic.AdditionalApproveNo1 = null;
                                //rentalSecurityBasic.AdditionalApproveNo2 = null;
                                //rentalSecurityBasic.AdditionalApproveNo3 = null;
                                //rentalSecurityBasic.MaintenanceFee1 = null;
                                //rentalSecurityBasic.MaintenanceFee2 = null;
                                rentalSecurityBasic.CompleteChangeOperationDate = null;
                                rentalSecurityBasic.CompleteChangeOperationEmpNo = null;
                                rentalSecurityBasic.SpecialInstallationFlag = null;
                                rentalSecurityBasic.PlannerEmpNo = null;
                                rentalSecurityBasic.PlanCheckerEmpNo = null;
                                rentalSecurityBasic.PlanCheckDate = null;
                                rentalSecurityBasic.PlanApproverEmpNo = null;
                                rentalSecurityBasic.PlanApproveDate = null;
                                rentalSecurityBasic.FacilityMemo = null;
                                rentalSecurityBasic.DocumentCode = null;
                                rentalSecurityBasic.DocAuditResult = null;
                                rentalSecurityBasic.SecurityMemo = null;
                                rentalSecurityBasic.ExpectedResumeDate = null; //Add by Jutarat A. on 02122013
                                rentalSecurityBasic.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                rentalSecurityBasic.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                rentalSecurityBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                rentalSecurityBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                dsRegisterRentalContract.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasic);
                            }

                            //Set data in dsEntireContract.dtTbt_RentalBEDetails
                            if (dsRentalContract.dtTbt_RentalBEDetails != null && dsRentalContract.dtTbt_RentalBEDetails.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                                tbt_RentalBEDetails rentalBEDetailsData;
                                foreach (tbt_RentalBEDetails data in dsRentalContract.dtTbt_RentalBEDetails)
                                {
                                    rentalBEDetailsData = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                                    rentalBEDetailsData.OCC = strNewImOCC;
                                    dsRegisterRentalContract.dtTbt_RentalBEDetails.Add(rentalBEDetailsData);
                                }
                            }

                            //Set data in dsEntireContract.dtTbt_RentalInstrumentDetails
                            if (dsRentalContract.dtTbt_RentalInstrumentDetails != null && dsRentalContract.dtTbt_RentalInstrumentDetails.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                                tbt_RentalInstrumentDetails rentalInstrumentDetailsData;
                                foreach (tbt_RentalInstrumentDetails data in dsRentalContract.dtTbt_RentalInstrumentDetails)
                                {
                                    rentalInstrumentDetailsData = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                                    rentalInstrumentDetailsData.OCC = strNewImOCC;
                                    dsRegisterRentalContract.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsData);
                                }
                            }

                            //Set data in dsEntireContract.dtTbt_RentalMaintenanceDetails
                            if (dsRentalContract.dtTbt_RentalMaintenanceDetails != null && dsRentalContract.dtTbt_RentalMaintenanceDetails.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                                tbt_RentalMaintenanceDetails rentalMaintenanceDetailsData;
                                foreach (tbt_RentalMaintenanceDetails data in dsRentalContract.dtTbt_RentalMaintenanceDetails)
                                {
                                    rentalMaintenanceDetailsData = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                                    rentalMaintenanceDetailsData.OCC = strNewImOCC;
                                    dsRegisterRentalContract.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsData);
                                }
                            }

                            //Set data in dsEntireContract.dtTbt_RentalOperationType
                            if (dsRentalContract.dtTbt_RentalOperationType != null && dsRentalContract.dtTbt_RentalOperationType.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                                tbt_RentalOperationType rentalOperationTypeData;
                                foreach (tbt_RentalOperationType data in dsRentalContract.dtTbt_RentalOperationType)
                                {
                                    rentalOperationTypeData = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(data);
                                    rentalOperationTypeData.OCC = strNewImOCC;
                                    dsRegisterRentalContract.dtTbt_RentalOperationType.Add(rentalOperationTypeData);
                                }
                            }

                            //Set data in dsEntireContract.dtTbt_RentalSentryGuard
                            if (dsRentalContract.dtTbt_RentalSentryGuard != null && dsRentalContract.dtTbt_RentalSentryGuard.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                                tbt_RentalSentryGuard rentalSentryGuardData;
                                foreach (tbt_RentalSentryGuard data in dsRentalContract.dtTbt_RentalSentryGuard)
                                {
                                    rentalSentryGuardData = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                                    rentalSentryGuardData.OCC = strNewImOCC;
                                    dsRegisterRentalContract.dtTbt_RentalSentryGuard.Add(rentalSentryGuardData);
                                }
                            }

                            //Set data in dsEntireContract.dtTbt_RentalSentryGuardDetails
                            if (dsRentalContract.dtTbt_RentalSentryGuardDetails != null && dsRentalContract.dtTbt_RentalSentryGuardDetails.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                                tbt_RentalSentryGuardDetails rentalSentryGuardDetailsData;
                                foreach (tbt_RentalSentryGuardDetails data in dsRentalContract.dtTbt_RentalSentryGuardDetails)
                                {
                                    rentalSentryGuardDetailsData = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                                    rentalSentryGuardDetailsData.OCC = strNewImOCC;
                                    dsRegisterRentalContract.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsData);
                                }
                            }

                            //Set data in dsEntireContract.dtTbt_RelationType
                            if (dsRentalContract.dtTbt_RelationType != null && dsRentalContract.dtTbt_RelationType.Count > 0)
                            {
                                dsRegisterRentalContract.dtTbt_RelationType = new List<tbt_RelationType>();
                                tbt_RelationType relationTypeData;
                                foreach (tbt_RelationType data in dsRentalContract.dtTbt_RelationType)
                                {
                                    relationTypeData = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                                    relationTypeData.OCC = strNewImOCC;
                                    dsRegisterRentalContract.dtTbt_RelationType.Add(relationTypeData);
                                }
                            }


                            //Save contract data
                            //dsRentalContractResult = rentralHandler.InsertEntireContract(dsRegisterRentalContract);
                            rentralHandler.InsertEntireContractForCTS010(dsRegisterRentalContract); //Modify by Jutarat A. on 19092013

                            //Set temporary parameter 
                            strHisOCC = strNewImOCC;
                        }

                    }
                    else
                    {
                        //Get rental contract basic data
                        doImpleTbtRentalContractBasic = rentralHandler.GetTbt_RentalContractBasic(sParam.ContractCode, null);
                        if (doImpleTbtRentalContractBasic != null && doImpleTbtRentalContractBasic.Count > 0)
                        {
                            doImpleTbtRentalContractBasic[0].ContractCode = sParam.ContractCode;
                            doImpleTbtRentalContractBasic[0].ContractTargetCustCode = doRegisCustomerTarget.doCustomer.CustCode;
                            doImpleTbtRentalContractBasic[0].ContractTargetSignerTypeCode = doRegisterData.ContractTargetSignerTypeCode;
                            doImpleTbtRentalContractBasic[0].RealCustomerCustCode = doRegisRealCustomer.doCustomer.CustCode;
                            doImpleTbtRentalContractBasic[0].BranchNameEN = doRegisterData.BranchNameEN;
                            doImpleTbtRentalContractBasic[0].BranchNameLC = doRegisterData.BranchNameLC;
                            doImpleTbtRentalContractBasic[0].BranchAddressEN = doRegisterData.BranchAddressEN;
                            doImpleTbtRentalContractBasic[0].BranchAddressLC = doRegisterData.BranchAddressLC;
                            doImpleTbtRentalContractBasic[0].ContactPoint = doRegisterData.ContactPoint;
                            doImpleTbtRentalContractBasic[0].SiteCode = doRegisRealCustomer.doSite.SiteCode;
                            doImpleTbtRentalContractBasic[0].LastChangeImplementDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; ;
                            doImpleTbtRentalContractBasic[0].ChangeNameProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; ;
                            //doImpleTbtRentalContractBasic[0].CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            //doImpleTbtRentalContractBasic[0].CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            //doImpleTbtRentalContractBasic[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            //doImpleTbtRentalContractBasic[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            //Set temporary parameter 
                            strHisOCC = doImpleTbtRentalContractBasic[0].LastOCC;

                            //Update rental contract basic (Implement data)
                            doImpleTbtRentalContractBasic = rentralHandler.UpdateTbt_RentalContractBasicCore(doImpleTbtRentalContractBasic[0]);
                        }
                    }
                    /*----------------------------------------*/

                    /*---Insert data to Contract customer history table ---*/
                    //Create data object
                    tbt_ContractCustomerHistory doTbt_ContractCustomerHistory = new tbt_ContractCustomerHistory();

                    //Set data to doTbt_ContractCustomerHistory
                    doTbt_ContractCustomerHistory.ContractCode = sParam.ContractCode;
                    //doTbt_ContractCustomerHistory.SequenceNo = //	Default = 1 Other Get MAX(SequenceNo)+1 of this contract code
                    doTbt_ContractCustomerHistory.ChangeDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_ContractCustomerHistory.OCC = strHisOCC;
                    doTbt_ContractCustomerHistory.ChangeNameReasonType = doRegisterData.ChangeNameReasonType;
                    doTbt_ContractCustomerHistory.BranchNameEN = doRegisterData.BranchNameEN;
                    doTbt_ContractCustomerHistory.BranchNameLC = doRegisterData.BranchNameLC;
                    doTbt_ContractCustomerHistory.BranchAddressEN = doRegisterData.BranchAddressEN;
                    doTbt_ContractCustomerHistory.BranchAddressLC = doRegisterData.BranchAddressLC;
                    doTbt_ContractCustomerHistory.ContractSignerTypeCode = doRegisterData.ContractTargetSignerTypeCode;

                    doTbt_ContractCustomerHistory.CSChangeFlag = blnCSChangeFlag;
                    doTbt_ContractCustomerHistory.CSCustCode = doRegisCustomerTarget.doCustomer.CustCode;
                    doTbt_ContractCustomerHistory.CSCustStatus = doRegisCustomerTarget.doCustomer.CustStatus;
                    doTbt_ContractCustomerHistory.CSImportantFlag = doRegisCustomerTarget.doCustomer.ImportantFlag;
                    doTbt_ContractCustomerHistory.CSCustNameEN = doRegisCustomerTarget.doCustomer.CustNameEN;
                    doTbt_ContractCustomerHistory.CSCustNameLC = doRegisCustomerTarget.doCustomer.CustNameLC;
                    doTbt_ContractCustomerHistory.CSCustFullNameEN = doRegisCustomerTarget.doCustomer.CustFullNameEN;
                    doTbt_ContractCustomerHistory.CSCustFullNameLC = doRegisCustomerTarget.doCustomer.CustFullNameLC;
                    doTbt_ContractCustomerHistory.CSRepPersonName = doRegisCustomerTarget.doCustomer.RepPersonName;
                    doTbt_ContractCustomerHistory.CSContactPersonName = doRegisCustomerTarget.doCustomer.ContactPersonName;
                    doTbt_ContractCustomerHistory.CSSECOMContactPerson = doRegisCustomerTarget.doCustomer.SECOMContactPerson;
                    doTbt_ContractCustomerHistory.CSCustTypeCode = doRegisCustomerTarget.doCustomer.CustTypeCode;
                    doTbt_ContractCustomerHistory.CSCompanyTypeCode = doRegisCustomerTarget.doCustomer.CompanyTypeCode;
                    doTbt_ContractCustomerHistory.CSFinancialMarketTypeCode = doRegisCustomerTarget.doCustomer.FinancialMarketTypeCode;
                    doTbt_ContractCustomerHistory.CSBusinessTypeCode = doRegisCustomerTarget.doCustomer.BusinessTypeCode;
                    doTbt_ContractCustomerHistory.CSPhoneNo = doRegisCustomerTarget.doCustomer.PhoneNo;
                    doTbt_ContractCustomerHistory.CSFaxNo = doRegisCustomerTarget.doCustomer.FaxNo;
                    doTbt_ContractCustomerHistory.CSIDNo = doRegisCustomerTarget.doCustomer.IDNo;
                    doTbt_ContractCustomerHistory.CSDummyIDFlag = doRegisCustomerTarget.doCustomer.DummyIDFlag;
                    doTbt_ContractCustomerHistory.CSRegionCode = doRegisCustomerTarget.doCustomer.RegionCode;
                    doTbt_ContractCustomerHistory.CSURL = doRegisCustomerTarget.doCustomer.URL;
                    doTbt_ContractCustomerHistory.CSMemo = doRegisCustomerTarget.doCustomer.Memo;
                    doTbt_ContractCustomerHistory.CSAddressEN = doRegisCustomerTarget.doCustomer.AddressEN;
                    doTbt_ContractCustomerHistory.CSAlleyEN = doRegisCustomerTarget.doCustomer.AlleyEN;
                    doTbt_ContractCustomerHistory.CSRoadEN = doRegisCustomerTarget.doCustomer.RoadEN;
                    doTbt_ContractCustomerHistory.CSSubDistrictEN = doRegisCustomerTarget.doCustomer.SubDistrictEN;
                    doTbt_ContractCustomerHistory.CSAddressFullEN = doRegisCustomerTarget.doCustomer.AddressFullEN;
                    doTbt_ContractCustomerHistory.CSAddressLC = doRegisCustomerTarget.doCustomer.AddressLC;
                    doTbt_ContractCustomerHistory.CSAlleyLC = doRegisCustomerTarget.doCustomer.AlleyLC;
                    doTbt_ContractCustomerHistory.CSRoadLC = doRegisCustomerTarget.doCustomer.RoadLC;
                    doTbt_ContractCustomerHistory.CSSubDistrictLC = doRegisCustomerTarget.doCustomer.SubDistrictLC;
                    doTbt_ContractCustomerHistory.CSAddressFullLC = doRegisCustomerTarget.doCustomer.AddressFullLC;
                    doTbt_ContractCustomerHistory.CSDistrictCode = doRegisCustomerTarget.doCustomer.DistrictCode;
                    doTbt_ContractCustomerHistory.CSProvinceCode = doRegisCustomerTarget.doCustomer.ProvinceCode;
                    doTbt_ContractCustomerHistory.CSZipCode = doRegisCustomerTarget.doCustomer.ZipCode;
                    //doTbt_ContractCustomerHistory.ContractSignerChangeFlag = blnContractSignerChangeFlag; //Add by Jutarat A. on 18092012

                    doTbt_ContractCustomerHistory.RCChangeFlag = blnRCChangeFlag;
                    doTbt_ContractCustomerHistory.RCCustCode = doRegisRealCustomer.doCustomer.CustCode;
                    doTbt_ContractCustomerHistory.RCCustStatus = doRegisRealCustomer.doCustomer.CustStatus;
                    doTbt_ContractCustomerHistory.RCImportantFlag = doRegisRealCustomer.doCustomer.ImportantFlag;
                    doTbt_ContractCustomerHistory.RCCustNameEN = doRegisRealCustomer.doCustomer.CustNameEN;
                    doTbt_ContractCustomerHistory.RCCustNameLC = doRegisRealCustomer.doCustomer.CustNameLC;
                    doTbt_ContractCustomerHistory.RCCustFullNameEN = doRegisRealCustomer.doCustomer.CustFullNameEN;
                    doTbt_ContractCustomerHistory.RCCustFullNameLC = doRegisRealCustomer.doCustomer.CustFullNameLC;
                    doTbt_ContractCustomerHistory.RCRepPersonName = doRegisRealCustomer.doCustomer.RepPersonName;
                    doTbt_ContractCustomerHistory.RCContactPersonName = doRegisRealCustomer.doCustomer.ContactPersonName;
                    doTbt_ContractCustomerHistory.RCSECOMContactPerson = doRegisRealCustomer.doCustomer.SECOMContactPerson;
                    doTbt_ContractCustomerHistory.RCCustTypeCode = doRegisRealCustomer.doCustomer.CustTypeCode;
                    doTbt_ContractCustomerHistory.RCCompanyTypeCode = doRegisRealCustomer.doCustomer.CompanyTypeCode;
                    doTbt_ContractCustomerHistory.RCFinancialMarketTypeCode = doRegisRealCustomer.doCustomer.FinancialMarketTypeCode;
                    doTbt_ContractCustomerHistory.RCBusinessTypeCode = doRegisRealCustomer.doCustomer.BusinessTypeCode;
                    doTbt_ContractCustomerHistory.RCPhoneNo = doRegisRealCustomer.doCustomer.PhoneNo;
                    doTbt_ContractCustomerHistory.RCFaxNo = doRegisRealCustomer.doCustomer.FaxNo;
                    doTbt_ContractCustomerHistory.RCIDNo = doRegisRealCustomer.doCustomer.IDNo;
                    doTbt_ContractCustomerHistory.RCDummyIDFlag = doRegisRealCustomer.doCustomer.DummyIDFlag;
                    doTbt_ContractCustomerHistory.RCRegionCode = doRegisRealCustomer.doCustomer.RegionCode;
                    doTbt_ContractCustomerHistory.RCURL = doRegisRealCustomer.doCustomer.URL;
                    doTbt_ContractCustomerHistory.RCMemo = doRegisRealCustomer.doCustomer.Memo;
                    doTbt_ContractCustomerHistory.RCAddressEN = doRegisRealCustomer.doCustomer.AddressEN;
                    doTbt_ContractCustomerHistory.RCAlleyEN = doRegisRealCustomer.doCustomer.AlleyEN;
                    doTbt_ContractCustomerHistory.RCRoadEN = doRegisRealCustomer.doCustomer.RoadEN;
                    doTbt_ContractCustomerHistory.RCSubDistrictEN = doRegisRealCustomer.doCustomer.SubDistrictEN;
                    doTbt_ContractCustomerHistory.RCAddressFullEN = doRegisRealCustomer.doCustomer.AddressFullEN;
                    doTbt_ContractCustomerHistory.RCAddressLC = doRegisRealCustomer.doCustomer.AddressLC;
                    doTbt_ContractCustomerHistory.RCAlleyLC = doRegisRealCustomer.doCustomer.AlleyLC;
                    doTbt_ContractCustomerHistory.RCRoadLC = doRegisRealCustomer.doCustomer.RoadLC;
                    doTbt_ContractCustomerHistory.RCSubDistrictLC = doRegisRealCustomer.doCustomer.SubDistrictLC;
                    doTbt_ContractCustomerHistory.RCAddressFullLC = doRegisRealCustomer.doCustomer.AddressFullLC;
                    doTbt_ContractCustomerHistory.RCDistrictCode = doRegisRealCustomer.doCustomer.DistrictCode;
                    doTbt_ContractCustomerHistory.RCProvinceCode = doRegisRealCustomer.doCustomer.ProvinceCode;
                    doTbt_ContractCustomerHistory.RCZipCode = doRegisRealCustomer.doCustomer.ZipCode;

                    doTbt_ContractCustomerHistory.SiteChangeFlag = blnSiteChangeFlag;
                    doTbt_ContractCustomerHistory.SiteCode = doRegisRealCustomer.doSite.SiteCode;
                    doTbt_ContractCustomerHistory.SiteCustCode = doRegisRealCustomer.doSite.CustCode;
                    doTbt_ContractCustomerHistory.SiteNo = doRegisRealCustomer.doSite.SiteNo;
                    doTbt_ContractCustomerHistory.SiteNameEN = doRegisRealCustomer.doSite.SiteNameEN;
                    doTbt_ContractCustomerHistory.SiteNameLC = doRegisRealCustomer.doSite.SiteNameLC;
                    doTbt_ContractCustomerHistory.SiteSECOMContactPerson = doRegisRealCustomer.doSite.SECOMContactPerson;
                    doTbt_ContractCustomerHistory.SitePersonInCharge = doRegisRealCustomer.doSite.PersonInCharge;
                    doTbt_ContractCustomerHistory.SitePhoneNo = doRegisRealCustomer.doSite.PhoneNo;
                    doTbt_ContractCustomerHistory.SiteBuildingUsageCode = doRegisRealCustomer.doSite.BuildingUsageCode;
                    doTbt_ContractCustomerHistory.SiteAddressEN = doRegisRealCustomer.doSite.AddressEN;
                    doTbt_ContractCustomerHistory.SiteAlleyEN = doRegisRealCustomer.doSite.AlleyEN;
                    doTbt_ContractCustomerHistory.SiteRoadEN = doRegisRealCustomer.doSite.RoadEN;
                    doTbt_ContractCustomerHistory.SiteSubDistrictEN = doRegisRealCustomer.doSite.SubDistrictEN;
                    doTbt_ContractCustomerHistory.SiteAddressFullEN = doRegisRealCustomer.doSite.AddressFullEN;
                    doTbt_ContractCustomerHistory.SiteAddressLC = doRegisRealCustomer.doSite.AddressLC;
                    doTbt_ContractCustomerHistory.SiteAlleyLC = doRegisRealCustomer.doSite.AlleyLC;
                    doTbt_ContractCustomerHistory.SiteRoadLC = doRegisRealCustomer.doSite.RoadLC;
                    doTbt_ContractCustomerHistory.SiteSubDistrictLC = doRegisRealCustomer.doSite.SubDistrictLC;
                    doTbt_ContractCustomerHistory.SiteAddressFullLC = doRegisRealCustomer.doSite.AddressFullLC;
                    doTbt_ContractCustomerHistory.SiteDistrictCode = doRegisRealCustomer.doSite.DistrictCode;
                    doTbt_ContractCustomerHistory.SiteProvinceCode = doRegisRealCustomer.doSite.ProvinceCode;
                    doTbt_ContractCustomerHistory.SiteZipCode = doRegisRealCustomer.doSite.ZipCode;

                    doTbt_ContractCustomerHistory.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_ContractCustomerHistory.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_ContractCustomerHistory.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_ContractCustomerHistory.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    //Insert data to contract customer history
                    contractHandler.InsertTbt_ContractCustomerHistory(new List<tbt_ContractCustomerHistory> { doTbt_ContractCustomerHistory });
                    /*----------------------------------------*/

                    scope.Complete();
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Register ChangeName and Address of Sale to database
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doRegisterData"></param>
        private void RegisterChangeNameAddressSale_CTS130(ObjectResultData res, CTS130_RegisterChangeNameAddressData doRegisterData)
        {
            List<doCustomerWithGroup> listPurchaser = null;
            List<doCustomerWithGroup> listRealCustomer = null;
            List<doSite> listSite = null;

            List<doCustomerWithGroup> tempListPurchaser = null;
            List<doCustomerWithGroup> tempListRealCustomer = null;
            List<doSite> tempListSite = null;

            List<doCustomer> doContractTargetPurchaserTempData = null;
            List<doCustomer> doRealCustomerTempData = null;

            doCustomerTarget doRegisCustomerTarget = null;
            doCustomerTarget doRegisRealCustomer = null;

            List<tbt_SaleBasic> doTbt_SaleBasic = null;
            tbt_SaleBasic doRegisterTbt_SaleBasic = new tbt_SaleBasic();
            List<tbt_SaleBasic> doTbt_SaleBasicResult = null;
            dsSaleContractData dsSaleContractOriginal = null;

            List<CTS110_BillingClientData> billingClientDataList;
            List<CTS110_BillingTargetData> billingTargetDataList;
            List<CTS110_BillingClientData> billingClientOriginalDataList;
            List<CTS110_BillingTargetData> billingTargetOriginalDataList;

            ICustomerMasterHandler cusMasterHandler;
            IBillingMasterHandler billMasterHandler;
            ISaleContractHandler saleHandler;
            ICommonContractHandler contractHandler;
            IBillingInterfaceHandler billInterfaceHandler;

            //Create temporary parameter 
            bool blnCSChangeFlag = false;
            //bool blnContractSignerChangeFlag = false; //Add by Jutarat A. on 18092012
            bool blnRCChangeFlag = false;
            bool blnSiteChangeFlag = false;

            try
            {
                CTS130_ScreenParameter sParam = GetScreenObject<CTS130_ScreenParameter>();
                listPurchaser = sParam.ContractTargetPurchaserData;
                listRealCustomer = sParam.RealCustomerData;
                listSite = sParam.SiteData;
                tempListPurchaser = sParam.ContractTargetPurchaserDataTemp;
                tempListRealCustomer = sParam.RealCustomerDataTemp;
                tempListSite = sParam.SiteDataTemp;
                billingClientDataList = sParam.BillingClientData;
                billingTargetDataList = sParam.BillingTargetData;
                billingClientOriginalDataList = sParam.BillingClientOriginalData;
                billingTargetOriginalDataList = sParam.BillingTargetOriginalData;

                dsSaleContractOriginal = sParam.SaleContractData;

                cusMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                contractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                billInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //Create data object doRegisCustomerTarget
                    doRegisCustomerTarget = new doCustomerTarget();

                    //Prepare data
                    doCustomer customerData = new doCustomer();
                    List<dtCustomerGroup> customerGroupData = new List<dtCustomerGroup>();
                    if (tempListPurchaser != null && tempListPurchaser.Count > 0)
                    {
                        customerData = CommonUtil.CloneObject<doCustomerWithGroup, doCustomer>(tempListPurchaser[0]);
                        customerGroupData = CommonUtil.ClonsObjectList<dtCustomeGroupData, dtCustomerGroup>(tempListPurchaser[0].CustomerGroupData);
                    }

                    doRegisCustomerTarget.doCustomer = customerData;
                    doRegisCustomerTarget.dtCustomerGroup = customerGroupData;
                    doRegisCustomerTarget.doSite = null;

                    //Get contract target data from customer master for comparing
                    if (tempListPurchaser != null && tempListPurchaser.Count > 0)
                    {
                        doContractTargetPurchaserTempData = cusMasterHandler.GetCustomer(tempListPurchaser[0].CustCode);
                        if (tempListPurchaser[0].CustCode == null || (doContractTargetPurchaserTempData != null && doContractTargetPurchaserTempData.Count > 0))
                        {
                            List<doCustomer> doContractTargetPurchaserModifyData = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomer>(tempListPurchaser);

                            //Manage contract target information
                            if (tempListPurchaser[0].CustCode == null || (CompareObjectData_CTS130(doContractTargetPurchaserModifyData[0], doContractTargetPurchaserTempData[0]) == false))
                            {
                                //Manage data
                                doRegisCustomerTarget = cusMasterHandler.ManageCustomerTarget(doRegisCustomerTarget);

                                //Set temporary parameter 
                                ////Add by Jutarat A. on 18092012
                                //if (dsSaleContractOriginal.dtTbt_SaleBasic != null
                                //    && dsSaleContractOriginal.dtTbt_SaleBasic[0].PurchaserSignerTypeCode != doRegisterData.ContractTargetSignerTypeCode)
                                //{
                                //    blnContractSignerChangeFlag = true;
                                //}
                                ////End Add
                                //else
                                //{
                                //    blnCSChangeFlag = true;
                                //}
                                blnCSChangeFlag = true;
                            }
                        }

                        //Update customer status
                        cusMasterHandler.ManageCustomerInformation(doRegisCustomerTarget.doCustomer.CustCode);

                        //Set temporary parameter in case contract target code or contract signer type is changed
                        if (sParam.SaleContractData != null && sParam.SaleContractData.dtTbt_SaleBasic != null
                            && sParam.SaleContractData.dtTbt_SaleBasic.Count > 0)
                        {
                            if (tempListPurchaser[0].CustCode != sParam.SaleContractData.dtTbt_SaleBasic[0].PurchaserCustCode
                                || doRegisterData.ContractTargetSignerTypeCode != sParam.SaleContractData.dtTbt_SaleBasic[0].PurchaserSignerTypeCode)
                            {
                                //Set temporary parameter
                                ////Add by Jutarat A. on 18092012
                                //if (dsSaleContractOriginal.dtTbt_SaleBasic != null
                                //    && dsSaleContractOriginal.dtTbt_SaleBasic[0].PurchaserSignerTypeCode != doRegisterData.ContractTargetSignerTypeCode)
                                //{
                                //    blnContractSignerChangeFlag = true;
                                //}
                                ////End Add
                                //else
                                //{
                                //    blnCSChangeFlag = true;
                                //}
                                blnCSChangeFlag = true;
                            }
                        }
                    }


                    //Create data object doRegisCustomerTarget
                    doRegisRealCustomer = new doCustomerTarget();

                    //Prepare data
                    customerData = new doCustomer();
                    customerGroupData = new List<dtCustomerGroup>();
                    if (tempListRealCustomer != null && tempListRealCustomer.Count > 0)
                    {
                        customerData = CommonUtil.CloneObject<doCustomerWithGroup, doCustomer>(tempListRealCustomer[0]);
                        customerGroupData = CommonUtil.ClonsObjectList<dtCustomeGroupData, dtCustomerGroup>(tempListRealCustomer[0].CustomerGroupData);
                    }

                    doSite siteData = new doSite();
                    if (sParam.SiteDataTemp != null && sParam.SiteDataTemp.Count > 0)
                    {
                        siteData = CommonUtil.CloneObject<doSite, doSite>(sParam.SiteDataTemp[0]);
                    }

                    doRegisRealCustomer.doCustomer = customerData;
                    doRegisRealCustomer.dtCustomerGroup = customerGroupData;
                    doRegisRealCustomer.doSite = siteData;

                    //Get real customer data from customer master for comparing
                    if (tempListRealCustomer != null && tempListRealCustomer.Count > 0)
                    {
                        doRealCustomerTempData = cusMasterHandler.GetCustomer(tempListRealCustomer[0].CustCode);
                        if (tempListRealCustomer[0].CustCode == null || (doRealCustomerTempData != null && doRealCustomerTempData.Count > 0))
                        {
                            List<doCustomer> doRealCustomerModifyData = CommonUtil.ClonsObjectList<doCustomerWithGroup, doCustomer>(tempListRealCustomer);

                            //Manage real customer information
                            if (tempListRealCustomer[0].CustCode == null || (CompareObjectData_CTS130(doRealCustomerModifyData[0], doRealCustomerTempData[0]) == false))
                            {
                                //Manage data
                                doRegisRealCustomer = cusMasterHandler.ManageCustomerTarget(doRegisRealCustomer);

                                //Set temporary parameter 
                                blnRCChangeFlag = true;
                            }
                            
                        }

                        //Update customer status
                        cusMasterHandler.ManageCustomerInformation(doRegisRealCustomer.doCustomer.CustCode);

                        //Set temporary parameter in case real customer code is changed
                        if (sParam.SaleContractData != null && sParam.SaleContractData.dtTbt_SaleBasic != null
                            && sParam.SaleContractData.dtTbt_SaleBasic.Count > 0)
                        {
                            if (tempListRealCustomer[0].CustCode != sParam.SaleContractData.dtTbt_SaleBasic[0].RealCustomerCustCode)
                            {
                                //Set temporary parameter 
                                blnRCChangeFlag = true;
                            }
                        }

                        //Set temporary parameter in site code is changed
                        if (sParam.SaleContractData != null && sParam.SaleContractData.dtTbt_SaleBasic != null
                            && sParam.SaleContractData.dtTbt_SaleBasic.Count > 0)
                        {
                            if (tempListSite[0].SiteCode != sParam.SaleContractData.dtTbt_SaleBasic[0].SiteCode)
                            {
                                //Set temporary parameter 
                                blnSiteChangeFlag = true;

                                if (tempListSite[0].SiteCode == null)
                                {
                                    //Manage data
                                    doRegisRealCustomer = cusMasterHandler.ManageCustomerTarget(doRegisRealCustomer);
                                }
                            }
                        }
                    }


                    //Create data object
                    tbm_BillingClient doRegisBillingClient = new tbm_BillingClient();
                    List<doBillingTempBasic> doRegisBillingBasicList = new List<doBillingTempBasic>();

                    //Modify by Jutarat A. on 24072012
                    /*--- Manage billing target in list
                    //Manage billing client
                    foreach (CTS110_BillingClientData billClient in billingClientDataList)
                    {
                        if (String.IsNullOrEmpty(billClient.BillingClientCode))
                        {
                            doRegisBillingClient = CommonUtil.CloneObject<CTS110_BillingClientData, tbm_BillingClient>(billClient);
                            billClient.BillingClientCode = billMasterHandler.ManageBillingClient(doRegisBillingClient);

                            foreach (CTS110_BillingTargetData target in billingTargetDataList)
                            {
                                if (billClient.Sequence == target.Sequence)
                                {
                                    target.BillingClientCode = billClient.BillingClientCode;
                                    break;
                                }
                            }
                        }
                    }

                    //Manage billing target
                    doBillingTempBasic doRegisBillingBasic;
                    foreach (CTS110_BillingTargetData orgData in billingTargetOriginalDataList)
                    {
                        foreach (CTS110_BillingTargetData newData in billingTargetDataList)
                        {
                            if (orgData.Sequence == newData.Sequence)
                            {
                                doRegisBillingBasic = new doBillingTempBasic();
                                if (orgData.BillingClientCode != newData.BillingClientCode
                                    || orgData.BillingOfficeCode != newData.BillingOfficeCode
                                    || orgData.BillingTargetCode != newData.BillingTargetCode)
                                {
                                    //Prepare data
                                    doRegisBillingBasic.ContractCode = sParam.ContractCode;
                                    doRegisBillingBasic.BillingOCC = newData.BillingOCC;
                                    doRegisBillingBasic.BillingClientCode = newData.BillingClientCode;
                                    doRegisBillingBasic.BillingOfficeCode = newData.BillingOfficeCode;
                                    doRegisBillingBasic.BillingTargetCode = newData.BillingTargetCode;

                                    doRegisBillingBasicList.Add(doRegisBillingBasic);
                                }
                            }
                        }
                    }
                    ---*/
                    //Manage billing client
                    if (CompareListData_CTS130<CTS110_BillingClientData>(billingClientOriginalDataList, billingClientDataList) == false)
                    {
                        foreach (CTS110_BillingClientData billClient in billingClientDataList)
                        {
                            if (String.IsNullOrEmpty(billClient.BillingClientCode))
                            {
                                doRegisBillingClient = CommonUtil.CloneObject<CTS110_BillingClientData, tbm_BillingClient>(billClient);
                                billClient.BillingClientCode = billMasterHandler.ManageBillingClient(doRegisBillingClient);

                                foreach (CTS110_BillingTargetData target in billingTargetDataList)
                                {
                                    if (billClient.Sequence == target.Sequence)
                                    {
                                        target.BillingClientCode = billClient.BillingClientCode;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //Manage billing target
                    if (CompareListData_CTS130<CTS110_BillingTargetData>(billingTargetOriginalDataList, billingTargetDataList) == false)
                    {
                        doBillingTempBasic doRegisBillingBasic;
                        foreach (CTS110_BillingTargetData newData in billingTargetDataList)
                        {
                            //Prepare data
                            doRegisBillingBasic = new doBillingTempBasic();
                            doRegisBillingBasic.ContractCode = sParam.ContractCode;
                            doRegisBillingBasic.BillingOCC = newData.BillingOCC;
                            doRegisBillingBasic.BillingClientCode = newData.BillingClientCode;
                            doRegisBillingBasic.BillingOfficeCode = newData.BillingOfficeCode;
                            doRegisBillingBasic.BillingTargetCode = newData.BillingTargetCode;

                            doRegisBillingBasicList.Add(doRegisBillingBasic);
                        }
                    }
                    //End Modify

                    //Send data to billing module
                    if (doRegisBillingBasicList != null && doRegisBillingBasicList.Count > 0)
                    {
                        billInterfaceHandler.SendBilling_ChangeName(doRegisBillingBasicList);
                    }
                    /*------------------------------------*/


                    /*--- Insert data to Sale contract DB ---*/
                    //Generate new OCC
                    string strNewOCC = saleHandler.GenerateContractOCC(sParam.ContractCode);

                    //Get last OCC
                    string strOCC = saleHandler.GetLastOCC(sParam.ContractCode);

                    //Get sale basic data
                    doTbt_SaleBasic = saleHandler.GetTbt_SaleBasic(sParam.ContractCode, strOCC, true);

                    //Update last OCC flag in sale basic
                    if (doTbt_SaleBasic != null && doTbt_SaleBasic.Count > 0)
                    {
                        //Check data updated by other person
                        if (dsSaleContractOriginal != null && dsSaleContractOriginal.dtTbt_SaleBasic != null && dsSaleContractOriginal.dtTbt_SaleBasic.Count > 0)
                        {
                            if (DateTime.Compare(doTbt_SaleBasic[0].UpdateDate.Value, dsSaleContractOriginal.dtTbt_SaleBasic[0].UpdateDate.Value) != 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            }
                        }

                        doTbt_SaleBasic[0].LatestOCCFlag = false;
                        doTbt_SaleBasic = saleHandler.UpdateTbt_SaleBasic(doTbt_SaleBasic[0]);
                    }

                    //Set data to doTbt_SaleBasic 
                    if (doTbt_SaleBasic != null && doTbt_SaleBasic.Count > 0)
                    {
                        doRegisterTbt_SaleBasic = CommonUtil.CloneObject<tbt_SaleBasic, tbt_SaleBasic>(doTbt_SaleBasic[0]);

                        doRegisterTbt_SaleBasic.ContractCode = sParam.ContractCode;
                        doRegisterTbt_SaleBasic.OCC = strNewOCC;
                        doRegisterTbt_SaleBasic.LatestOCCFlag = true;
                        doRegisterTbt_SaleBasic.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_CHANGE_NAME;
                        doRegisterTbt_SaleBasic.SaleProcessManageStatus = null;
                        doRegisterTbt_SaleBasic.CounterNo = 0;
                        doRegisterTbt_SaleBasic.PurchaserCustCode = doRegisCustomerTarget.doCustomer.CustCode;
                        doRegisterTbt_SaleBasic.PurchaserSignerTypeCode = doRegisterData.ContractTargetSignerTypeCode;
                        doRegisterTbt_SaleBasic.RealCustomerCustCode = doRegisRealCustomer.doCustomer.CustCode;
                        doRegisterTbt_SaleBasic.BranchNameEN = doRegisterData.BranchNameEN;
                        doRegisterTbt_SaleBasic.BranchNameLC = doRegisterData.BranchNameLC;
                        doRegisterTbt_SaleBasic.BranchAddressEN = doRegisterData.BranchAddressEN;
                        doRegisterTbt_SaleBasic.BranchAddressLC = doRegisterData.BranchAddressLC;
                        doRegisterTbt_SaleBasic.ContactPoint = doRegisterData.ContactPoint;
                        doRegisterTbt_SaleBasic.SiteCode = doRegisRealCustomer.doSite.SiteCode;
                        doRegisterTbt_SaleBasic.InstallationCompleteFlag = null;
                        doRegisterTbt_SaleBasic.MaintenanceContractFlag = null;
                        doRegisterTbt_SaleBasic.LastChangeProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doRegisterTbt_SaleBasic.ExpectedInstallCompleteDate = null;
                        doRegisterTbt_SaleBasic.ExpectedCustAcceptanceDate = null;
                        doRegisterTbt_SaleBasic.InstrumentStockOutDate = null;
                        doRegisterTbt_SaleBasic.SubcontractInstallCompleteDate = null;
                        doRegisterTbt_SaleBasic.InstallCompleteProcessDate = null;
                        doRegisterTbt_SaleBasic.InstallCompleteDate = null;
                        doRegisterTbt_SaleBasic.CustAcceptanceDate = null;
                        doRegisterTbt_SaleBasic.DeliveryDocReceiveDate = null;
                        doRegisterTbt_SaleBasic.DataCorrectionProcessDate = null;
                        doRegisterTbt_SaleBasic.DataCorrectionProcessEmpNo = null;
                        doRegisterTbt_SaleBasic.WarranteeFrom = null;
                        doRegisterTbt_SaleBasic.WarranteeTo = null;
                        doRegisterTbt_SaleBasic.StartMaintenanceDate = null;
                        doRegisterTbt_SaleBasic.EndMaintenanceDate = null;
                        doRegisterTbt_SaleBasic.NormalProductPrice = null;
                        doRegisterTbt_SaleBasic.NormalInstallFee = null;
                        doRegisterTbt_SaleBasic.NormalSalePrice = null;
                        doRegisterTbt_SaleBasic.OrderProductPrice = null;
                        doRegisterTbt_SaleBasic.OrderInstallFee = null;
                        doRegisterTbt_SaleBasic.OrderSalePrice = null;
                        doRegisterTbt_SaleBasic.BillingAmt_ApproveContract = null;
                        doRegisterTbt_SaleBasic.BillingAmt_PartialFee = null;
                        doRegisterTbt_SaleBasic.BillingAmt_Acceptance = null;
                        doRegisterTbt_SaleBasic.SaleAdjAmt = null;
                        doRegisterTbt_SaleBasic.QuotationTargetCode = null;
                        doRegisterTbt_SaleBasic.Alphabet = null;
                        doRegisterTbt_SaleBasic.NegotiationStaffEmpNo1 = null;
                        doRegisterTbt_SaleBasic.NegotiationStaffEmpNo2 = null;
                        doRegisterTbt_SaleBasic.ApproveNo1 = null;
                        doRegisterTbt_SaleBasic.ApproveNo2 = null;
                        doRegisterTbt_SaleBasic.ApproveNo3 = null;
                        doRegisterTbt_SaleBasic.ApproveNo4 = null;
                        doRegisterTbt_SaleBasic.ApproveNo5 = null;
                        doRegisterTbt_SaleBasic.IEInchargeEmpNo = null;
                        doRegisterTbt_SaleBasic.InstallationCompleteEmpNo = null;
                        doRegisterTbt_SaleBasic.InstallationSlipNo = null;
                        doRegisterTbt_SaleBasic.InstallationTypeCode = null;
                        doRegisterTbt_SaleBasic.InstallFeePaidBySECOM = null;
                        doRegisterTbt_SaleBasic.InstallFeeRevenueBySECOM = null;
                        doRegisterTbt_SaleBasic.ChangeNameProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doRegisterTbt_SaleBasic.ContractConditionProcessDate = null;
                        doRegisterTbt_SaleBasic.ContractConditionProcessEmpNo = null;
                        doRegisterTbt_SaleBasic.AlmightyProgramprocessDate = null;
                        doRegisterTbt_SaleBasic.AlmightyProgramEmpNo = null;
                        doRegisterTbt_SaleBasic.DocReceiveDate = null;
                        doRegisterTbt_SaleBasic.DocAuditResult = null;
                        doRegisterTbt_SaleBasic.DocumentCode = null;
                        doRegisterTbt_SaleBasic.BidGuaranteeAmount1 = null;
                        doRegisterTbt_SaleBasic.BidGuaranteeAmount2 = null;
                        doRegisterTbt_SaleBasic.MotivationTypeCode = null;
                        doRegisterTbt_SaleBasic.InstallFeeType = null;
                        doRegisterTbt_SaleBasic.ChangeNameReasonType = doRegisterData.ChangeNameReasonType;
                        doRegisterTbt_SaleBasic.PlanCheckerEmpNo = null;
                        doRegisterTbt_SaleBasic.PlanCheckDate = null;
                        doRegisterTbt_SaleBasic.PlanApproverEmpNo = null;
                        doRegisterTbt_SaleBasic.PlanApproveDate = null;
                        doRegisterTbt_SaleBasic.CancelReasonType = null;
                        doRegisterTbt_SaleBasic.CancelDate = null;
                        doRegisterTbt_SaleBasic.CancelProcessDate = null;
                        //doRegisterTbt_SaleBasic.ChangeImplementDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //Comment by Jutarat A. on 10042013 (Not change)
                        doRegisterTbt_SaleBasic.NewAddInstallCompleteProcessDate = null;
                        doRegisterTbt_SaleBasic.NewAddInstallCompleteEmpNo = null;
                        doRegisterTbt_SaleBasic.BICContractCode = null;
                        doRegisterTbt_SaleBasic.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doRegisterTbt_SaleBasic.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doRegisterTbt_SaleBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doRegisterTbt_SaleBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        //Insert data to sale basic table
                        doTbt_SaleBasicResult = saleHandler.InsertTbt_SaleBasic(doRegisterTbt_SaleBasic);
                    }
                    /*----------------------------------------*/

                    //Insert data to Contract customer history table  
                    //Create data object
                    tbt_ContractCustomerHistory doTbt_ContractCustomerHistory = new tbt_ContractCustomerHistory();

                    //Set data to doTbt_ContractCustomerHistory
                    doTbt_ContractCustomerHistory.ContractCode = sParam.ContractCode;
                    //doTbt_ContractCustomerHistory.SequenceNo = //	Default = 1 Other Get MAX(SequenceNo)+1 of this contract code
                    doTbt_ContractCustomerHistory.ChangeDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_ContractCustomerHistory.OCC = strNewOCC;
                    doTbt_ContractCustomerHistory.ChangeNameReasonType = doRegisterData.ChangeNameReasonType;
                    doTbt_ContractCustomerHistory.BranchNameEN = doRegisterData.BranchNameEN;
                    doTbt_ContractCustomerHistory.BranchNameLC = doRegisterData.BranchNameLC;
                    doTbt_ContractCustomerHistory.BranchAddressEN = doRegisterData.BranchAddressEN;
                    doTbt_ContractCustomerHistory.BranchAddressLC = doRegisterData.BranchAddressLC;
                    doTbt_ContractCustomerHistory.ContractSignerTypeCode = doRegisterData.ContractTargetSignerTypeCode;

                    doTbt_ContractCustomerHistory.CSChangeFlag = blnCSChangeFlag;
                    doTbt_ContractCustomerHistory.CSCustCode = doRegisCustomerTarget.doCustomer.CustCode;
                    doTbt_ContractCustomerHistory.CSCustStatus = doRegisCustomerTarget.doCustomer.CustStatus;
                    doTbt_ContractCustomerHistory.CSImportantFlag = doRegisCustomerTarget.doCustomer.ImportantFlag;
                    doTbt_ContractCustomerHistory.CSCustNameEN = doRegisCustomerTarget.doCustomer.CustNameEN;
                    doTbt_ContractCustomerHistory.CSCustNameLC = doRegisCustomerTarget.doCustomer.CustNameLC;
                    doTbt_ContractCustomerHistory.CSCustFullNameEN = doRegisCustomerTarget.doCustomer.CustFullNameEN;
                    doTbt_ContractCustomerHistory.CSCustFullNameLC = doRegisCustomerTarget.doCustomer.CustFullNameLC;
                    doTbt_ContractCustomerHistory.CSRepPersonName = doRegisCustomerTarget.doCustomer.RepPersonName;
                    doTbt_ContractCustomerHistory.CSContactPersonName = doRegisCustomerTarget.doCustomer.ContactPersonName;
                    doTbt_ContractCustomerHistory.CSSECOMContactPerson = doRegisCustomerTarget.doCustomer.SECOMContactPerson;
                    doTbt_ContractCustomerHistory.CSCustTypeCode = doRegisCustomerTarget.doCustomer.CustTypeCode;
                    doTbt_ContractCustomerHistory.CSCompanyTypeCode = doRegisCustomerTarget.doCustomer.CompanyTypeCode;
                    doTbt_ContractCustomerHistory.CSFinancialMarketTypeCode = doRegisCustomerTarget.doCustomer.FinancialMarketTypeCode;
                    doTbt_ContractCustomerHistory.CSBusinessTypeCode = doRegisCustomerTarget.doCustomer.BusinessTypeCode;
                    doTbt_ContractCustomerHistory.CSPhoneNo = doRegisCustomerTarget.doCustomer.PhoneNo;
                    doTbt_ContractCustomerHistory.CSFaxNo = doRegisCustomerTarget.doCustomer.FaxNo;
                    doTbt_ContractCustomerHistory.CSIDNo = doRegisCustomerTarget.doCustomer.IDNo;
                    doTbt_ContractCustomerHistory.CSDummyIDFlag = doRegisCustomerTarget.doCustomer.DummyIDFlag;
                    doTbt_ContractCustomerHistory.CSRegionCode = doRegisCustomerTarget.doCustomer.RegionCode;
                    doTbt_ContractCustomerHistory.CSURL = doRegisCustomerTarget.doCustomer.URL;
                    doTbt_ContractCustomerHistory.CSMemo = doRegisCustomerTarget.doCustomer.Memo;
                    doTbt_ContractCustomerHistory.CSAddressEN = doRegisCustomerTarget.doCustomer.AddressEN;
                    doTbt_ContractCustomerHistory.CSAlleyEN = doRegisCustomerTarget.doCustomer.AlleyEN;
                    doTbt_ContractCustomerHistory.CSRoadEN = doRegisCustomerTarget.doCustomer.RoadEN;
                    doTbt_ContractCustomerHistory.CSSubDistrictEN = doRegisCustomerTarget.doCustomer.SubDistrictEN;
                    doTbt_ContractCustomerHistory.CSAddressFullEN = doRegisCustomerTarget.doCustomer.AddressFullEN;
                    doTbt_ContractCustomerHistory.CSAddressLC = doRegisCustomerTarget.doCustomer.AddressLC;
                    doTbt_ContractCustomerHistory.CSAlleyLC = doRegisCustomerTarget.doCustomer.AlleyLC;
                    doTbt_ContractCustomerHistory.CSRoadLC = doRegisCustomerTarget.doCustomer.RoadLC;
                    doTbt_ContractCustomerHistory.CSSubDistrictLC = doRegisCustomerTarget.doCustomer.SubDistrictLC;
                    doTbt_ContractCustomerHistory.CSAddressFullLC = doRegisCustomerTarget.doCustomer.AddressFullLC;
                    doTbt_ContractCustomerHistory.CSDistrictCode = doRegisCustomerTarget.doCustomer.DistrictCode;
                    doTbt_ContractCustomerHistory.CSProvinceCode = doRegisCustomerTarget.doCustomer.ProvinceCode;
                    doTbt_ContractCustomerHistory.CSZipCode = doRegisCustomerTarget.doCustomer.ZipCode;
                    //doTbt_ContractCustomerHistory.ContractSignerChangeFlag = blnContractSignerChangeFlag; //Add by Jutarat A. on 18092012

                    doTbt_ContractCustomerHistory.RCChangeFlag = blnRCChangeFlag;
                    doTbt_ContractCustomerHistory.RCCustCode = doRegisRealCustomer.doCustomer.CustCode;
                    doTbt_ContractCustomerHistory.RCCustStatus = doRegisRealCustomer.doCustomer.CustStatus;
                    doTbt_ContractCustomerHistory.RCImportantFlag = doRegisRealCustomer.doCustomer.ImportantFlag;
                    doTbt_ContractCustomerHistory.RCCustNameEN = doRegisRealCustomer.doCustomer.CustNameEN;
                    doTbt_ContractCustomerHistory.RCCustNameLC = doRegisRealCustomer.doCustomer.CustNameLC;
                    doTbt_ContractCustomerHistory.RCCustFullNameEN = doRegisRealCustomer.doCustomer.CustFullNameEN;
                    doTbt_ContractCustomerHistory.RCCustFullNameLC = doRegisRealCustomer.doCustomer.CustFullNameLC;
                    doTbt_ContractCustomerHistory.RCRepPersonName = doRegisRealCustomer.doCustomer.RepPersonName;
                    doTbt_ContractCustomerHistory.RCContactPersonName = doRegisRealCustomer.doCustomer.ContactPersonName;
                    doTbt_ContractCustomerHistory.RCSECOMContactPerson = doRegisRealCustomer.doCustomer.SECOMContactPerson;
                    doTbt_ContractCustomerHistory.RCCustTypeCode = doRegisRealCustomer.doCustomer.CustTypeCode;
                    doTbt_ContractCustomerHistory.RCCompanyTypeCode = doRegisRealCustomer.doCustomer.CompanyTypeCode;
                    doTbt_ContractCustomerHistory.RCFinancialMarketTypeCode = doRegisRealCustomer.doCustomer.FinancialMarketTypeCode;
                    doTbt_ContractCustomerHistory.RCBusinessTypeCode = doRegisRealCustomer.doCustomer.BusinessTypeCode;
                    doTbt_ContractCustomerHistory.RCPhoneNo = doRegisRealCustomer.doCustomer.PhoneNo;
                    doTbt_ContractCustomerHistory.RCFaxNo = doRegisRealCustomer.doCustomer.FaxNo;
                    doTbt_ContractCustomerHistory.RCIDNo = doRegisRealCustomer.doCustomer.IDNo;
                    doTbt_ContractCustomerHistory.RCDummyIDFlag = doRegisRealCustomer.doCustomer.DummyIDFlag;
                    doTbt_ContractCustomerHistory.RCRegionCode = doRegisRealCustomer.doCustomer.RegionCode;
                    doTbt_ContractCustomerHistory.RCURL = doRegisRealCustomer.doCustomer.URL;
                    doTbt_ContractCustomerHistory.RCMemo = doRegisRealCustomer.doCustomer.Memo;
                    doTbt_ContractCustomerHistory.RCAddressEN = doRegisRealCustomer.doCustomer.AddressEN;
                    doTbt_ContractCustomerHistory.RCAlleyEN = doRegisRealCustomer.doCustomer.AlleyEN;
                    doTbt_ContractCustomerHistory.RCRoadEN = doRegisRealCustomer.doCustomer.RoadEN;
                    doTbt_ContractCustomerHistory.RCSubDistrictEN = doRegisRealCustomer.doCustomer.SubDistrictEN;
                    doTbt_ContractCustomerHistory.RCAddressFullEN = doRegisRealCustomer.doCustomer.AddressFullEN;
                    doTbt_ContractCustomerHistory.RCAddressLC = doRegisRealCustomer.doCustomer.AddressLC;
                    doTbt_ContractCustomerHistory.RCAlleyLC = doRegisRealCustomer.doCustomer.AlleyLC;
                    doTbt_ContractCustomerHistory.RCRoadLC = doRegisRealCustomer.doCustomer.RoadLC;
                    doTbt_ContractCustomerHistory.RCSubDistrictLC = doRegisRealCustomer.doCustomer.SubDistrictLC;
                    doTbt_ContractCustomerHistory.RCAddressFullLC = doRegisRealCustomer.doCustomer.AddressFullLC;
                    doTbt_ContractCustomerHistory.RCDistrictCode = doRegisRealCustomer.doCustomer.DistrictCode;
                    doTbt_ContractCustomerHistory.RCProvinceCode = doRegisRealCustomer.doCustomer.ProvinceCode;
                    doTbt_ContractCustomerHistory.RCZipCode = doRegisRealCustomer.doCustomer.ZipCode;

                    doTbt_ContractCustomerHistory.SiteChangeFlag = blnSiteChangeFlag;
                    doTbt_ContractCustomerHistory.SiteCode = doRegisRealCustomer.doSite.SiteCode;
                    doTbt_ContractCustomerHistory.SiteCustCode = doRegisRealCustomer.doSite.CustCode;
                    doTbt_ContractCustomerHistory.SiteNo = doRegisRealCustomer.doSite.SiteNo;
                    doTbt_ContractCustomerHistory.SiteNameEN = doRegisRealCustomer.doSite.SiteNameEN;
                    doTbt_ContractCustomerHistory.SiteNameLC = doRegisRealCustomer.doSite.SiteNameLC;
                    doTbt_ContractCustomerHistory.SiteSECOMContactPerson = doRegisRealCustomer.doSite.SECOMContactPerson;
                    doTbt_ContractCustomerHistory.SitePersonInCharge = doRegisRealCustomer.doSite.PersonInCharge;
                    doTbt_ContractCustomerHistory.SitePhoneNo = doRegisRealCustomer.doSite.PhoneNo;
                    doTbt_ContractCustomerHistory.SiteBuildingUsageCode = doRegisRealCustomer.doSite.BuildingUsageCode;
                    doTbt_ContractCustomerHistory.SiteAddressEN = doRegisRealCustomer.doSite.AddressEN;
                    doTbt_ContractCustomerHistory.SiteAlleyEN = doRegisRealCustomer.doSite.AlleyEN;
                    doTbt_ContractCustomerHistory.SiteRoadEN = doRegisRealCustomer.doSite.RoadEN;
                    doTbt_ContractCustomerHistory.SiteSubDistrictEN = doRegisRealCustomer.doSite.SubDistrictEN;
                    doTbt_ContractCustomerHistory.SiteAddressFullEN = doRegisRealCustomer.doSite.AddressFullEN;
                    doTbt_ContractCustomerHistory.SiteAddressLC = doRegisRealCustomer.doSite.AddressLC;
                    doTbt_ContractCustomerHistory.SiteAlleyLC = doRegisRealCustomer.doSite.AlleyLC;
                    doTbt_ContractCustomerHistory.SiteRoadLC = doRegisRealCustomer.doSite.RoadLC;
                    doTbt_ContractCustomerHistory.SiteSubDistrictLC = doRegisRealCustomer.doSite.SubDistrictLC;
                    doTbt_ContractCustomerHistory.SiteAddressFullLC = doRegisRealCustomer.doSite.AddressFullLC;
                    doTbt_ContractCustomerHistory.SiteDistrictCode = doRegisRealCustomer.doSite.DistrictCode;
                    doTbt_ContractCustomerHistory.SiteProvinceCode = doRegisRealCustomer.doSite.ProvinceCode;
                    doTbt_ContractCustomerHistory.SiteZipCode = doRegisRealCustomer.doSite.ZipCode;

                    doTbt_ContractCustomerHistory.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_ContractCustomerHistory.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_ContractCustomerHistory.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_ContractCustomerHistory.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    //Insert data to contract customer history
                    contractHandler.InsertTbt_ContractCustomerHistory(new List<tbt_ContractCustomerHistory> { doTbt_ContractCustomerHistory });

                    scope.Complete();
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Compare data of Object
        /// </summary>
        /// <param name="objParam1"></param>
        /// <param name="objParam2"></param>
        /// <returns></returns>
        private bool CompareObjectData_CTS130(object objParam1, object objParam2)
        {
            bool isMatch = true;

            try
            {
                if (objParam1 != null && objParam2 != null
                    && objParam1.GetType() == objParam2.GetType())
                {
                    object obj1;
                    object obj2;

                    PropertyInfo[] propInfoList = objParam1.GetType().GetProperties();
                    foreach (PropertyInfo prop in propInfoList)
                    {
                        obj1 = prop.GetValue(objParam1, null);
                        obj2 = prop.GetValue(objParam2, null);
                        if ((obj1 == null && obj2 != null) || (obj1 != null && obj2 == null))
                        {
                            isMatch = false;
                        }
                        else if (obj1 == null && obj2 == null)
                        {
                            isMatch = true;   
                        }
                        else
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                isMatch = (String.Compare((string)obj1, (string)obj2) == 0);
                            }
                            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                            {
                                isMatch = (DateTime.Compare((DateTime)obj1, (DateTime)obj2) == 0);
                            }
                            else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                            {
                                isMatch = (((bool)obj1).CompareTo((bool)obj2) == 0);
                            }
                            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                            {
                                isMatch = ((int)obj1 == (int)obj2);
                            }
                            else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                            {
                                isMatch = ((decimal)obj1 == (decimal)obj2);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (isMatch == false)
                            break;
                    }
                }
                else
                {
                    isMatch = false;
                }

                return isMatch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Compare data of List
        /// </summary>
        /// <param name="objParam1"></param>
        /// <param name="objParam2"></param>
        /// <returns></returns>
        private bool CompareListData_CTS130<T>(List<T> objParam1, List<T> objParam2) where T : class
        {
            bool isMatch = true;

            try
            {
                if (objParam1 != null && objParam2 != null)
                {
                    foreach (T obj1 in objParam1)
                    {
                        foreach (T obj2 in objParam2)
                        {
                            isMatch = CompareObjectData_CTS130(obj1, obj2);
                            if (isMatch == false)
                                break;
                        }
                    }
                }
                else
                {
                    isMatch = false;
                }

                return isMatch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
