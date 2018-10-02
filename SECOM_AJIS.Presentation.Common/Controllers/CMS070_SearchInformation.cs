//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

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

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Common.Models;


namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        private const string CMS070_Screen = "CMS070";

        /// <summary>
        /// Check user permission for screen CMS070.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult CMS070_Authority(CMS070_ScreenParameter obj)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_INFORMATION, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                obj.hasPermission_CMS080 = CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_INFO, FunctionID.C_FUNC_ID_OPERATE);
                obj.hasPermission_CMS190 = CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CONTRACT_DIGEST, FunctionID.C_FUNC_ID_OPERATE);
                obj.hasPermission_CMS280 = CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_SITE_INFO, FunctionID.C_FUNC_ID_OPERATE);

                return InitialScreenEnvironment<CMS070_ScreenParameter>(CMS070_Screen, obj, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen CMS070.
        /// </summary>
        /// <returns></returns>
        [Initialize(CMS070_Screen)]
        public ActionResult CMS070()
        {
            CMS070_ScreenParameter param = new CMS070_ScreenParameter();
            ViewBag.HasPermissionCMS080 = "";
            ViewBag.HasPermissionCMS190 = "";
            ViewBag.HasPermissionCMS280 = "";
            ViewBag.CustomerRoleRealCust = "";

            try
            {
                param = GetScreenObject<CMS070_ScreenParameter>();
                ViewBag.HasPermissionCMS080 = param.hasPermission_CMS080;
                ViewBag.HasPermissionCMS190 = param.hasPermission_CMS190;
                ViewBag.HasPermissionCMS280 = param.hasPermission_CMS280;
                ViewBag.CustomerRoleRealCust = CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST;
                ViewBag.radioDefault = param.radioDefault;
            }
            catch
            {
            }

            return View();
        }

        /// <summary>
        /// Get config for Customer List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS070_InitCustomerGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS070_customer"));
        }

        /// <summary>
        /// Get config for Contract List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS070_InitContractGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS070_contract"));
        }

        /// <summary>
        /// Validate search criteria in case user choose search customer by code
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public ActionResult CMS070_ValidateCustomerSearchByCode(CMS070_CustomerByCode searchCondition)
        {
            ObjectResultData res = new ObjectResultData();
            List<MessageModel> wLst = new List<MessageModel>();
            try
            {
                //3.1	Validate require field
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { searchCondition });

                if (res.IsError)
                {
                    return Json(res);
                }

                res.ResultData = "P";

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Validate search criteria in case user choose search contract by code
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public ActionResult CMS070_ValidateContractSearchByCode(CMS070_ContractByCode searchCondition)
        {
            ObjectResultData res = new ObjectResultData();
            List<MessageModel> wLst = new List<MessageModel>();
            try
            {
                //3.1	Validate require field
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { searchCondition });

                if (res.IsError)
                {
                    return Json(res);
                }

                res.ResultData = "P";
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Validate search criteria in case user choose search by condition
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public ActionResult CMS070_ValidateSearchByCondition(doSearchInfoCondition searchCondition)
        {
            ObjectResultData res = new ObjectResultData();
            List<MessageModel> wLst = new List<MessageModel>();
            try
            {
                //3.1	Validate require field



                //----- New requirement from costomer -> not validate this field (CustomerRole)---- 
                //----- Narupon W. 27/Feb/2012-----------------------------------------------------
                //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //if (searchCondition.CustomerRole == null || searchCondition.CustomerRole.Equals("")) {
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0089);
                //}



                //if (searchCondition.CustomerStatusExist == null && searchCondition.CustomerStatusNew == null) {
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0054);
                //}

                if (searchCondition.CustTypeAssociation == null && searchCondition.CustTypeIndividual == null && searchCondition.CustTypeJuristic == null
                     && searchCondition.CustTypeOther == null && searchCondition.CustTypePublicOffice == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0055);
                }

                if ((searchCondition.OperationDateFrom.HasValue && searchCondition.OperationDateTo.HasValue)
                    && searchCondition.OperationDateFrom.Value.CompareTo(searchCondition.OperationDateTo.Value) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0090);
                }

                if ((searchCondition.CustAcceptDateFrom.HasValue && searchCondition.CustAcceptDateTo.HasValue)
                   && searchCondition.CustAcceptDateFrom.Value.CompareTo(searchCondition.CustAcceptDateTo.Value) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0090);
                }
                if ((searchCondition.CompleteDateFrom.HasValue && searchCondition.CompleteDateTo.HasValue)
                    && searchCondition.CompleteDateFrom.Value.CompareTo(searchCondition.CompleteDateTo.Value) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0090);
                }

                if (!res.IsError)
                {
                    res.ResultData = "P";
                }
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Search customer data by search criteria.
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public ActionResult CMS070_CustomerSearch(doSearchInfoCondition searchCondition)
        {
            ObjectResultData res = new ObjectResultData();
            string customerStatus = ",";
            string customerType = ",";
            try
            {
                //customerStatus += searchCondition.CustomerStatusNew == null ? "" : CustomerStatus.C_CUST_STATUS_NEW + ",";
                //customerStatus += searchCondition.CustomerStatusExist == null ? "" : CustomerStatus.C_CUST_STATUS_EXIST + ",";
                //if (customerStatus.Equals(","))
                customerStatus = null;

                customerType += searchCondition.CustTypeJuristic == null ? "" : CustomerType.C_CUST_TYPE_JURISTIC + ",";
                customerType += searchCondition.CustTypeIndividual == null ? "" : CustomerType.C_CUST_TYPE_INDIVIDUAL + ",";
                customerType += searchCondition.CustTypeAssociation == null ? "" : CustomerType.C_CUST_TYPE_ASSOCIATION + ",";
                customerType += searchCondition.CustTypePublicOffice == null ? "" : CustomerType.C_CUST_TYPE_PUBLIC_OFFICE + ",";
                customerType += searchCondition.CustTypeOther == null ? "" : CustomerType.C_CUST_TYPE_OTHERS + ",";
                if (customerType.Equals(","))
                    customerType = null;

                string RoleTypeContractTarget = null;
                string RoleTypePurchaser = null;
                string RoleTypeRealCustomer = null;

                if (CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET.Equals(searchCondition.CustomerRole))
                {
                    RoleTypeContractTarget = searchCondition.CustomerRole;
                }
                else if (CustRoleType.C_CUST_ROLE_TYPE_PURCHASER.Equals(searchCondition.CustomerRole))
                {
                    RoleTypePurchaser = searchCondition.CustomerRole;
                }
                else if (CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST.Equals(searchCondition.CustomerRole))
                {
                    RoleTypeRealCustomer = searchCondition.CustomerRole;
                }

                CommonUtil c = new CommonUtil();
                searchCondition.CustomerCode = c.ConvertCustCode(searchCondition.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                searchCondition.GroupCode = c.ConvertGroupCode(searchCondition.GroupCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtCustomerList> list = hand.GetCustomerListForSearchInfo(
                    searchCondition.CustomerCode,
                    RoleTypeContractTarget,
                    RoleTypePurchaser,
                    RoleTypeRealCustomer,
                    searchCondition.GroupCode,
                    searchCondition.CustomerName,
                    //searchCondition.Branchename,
                    searchCondition.GroupName,
                    customerStatus,
                    customerType,
                    searchCondition.CompanyType,
                    searchCondition.CustomerIDNo,
                    searchCondition.CustomerNatioality,
                    searchCondition.CustomerBusinessType,
                    searchCondition.CustomerAddress,
                    searchCondition.CustomerSoi,
                    searchCondition.CustomerRoad,
                    searchCondition.CustomerTumbol,
                    searchCondition.CustomerJangwat,
                    searchCondition.CustomerAmper,
                    searchCondition.CustomerZipCode,
                    searchCondition.CustomerTelephone,
                    searchCondition.CustomerFax,
                    CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET,
                    CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST,
                    CustRoleType.C_CUST_ROLE_TYPE_PURCHASER,
                    FlagType.C_FLAG_ON,
                    ServiceType.C_SERVICE_TYPE_RENTAL,
                    ServiceType.C_SERVICE_TYPE_SALE,
                    ContractStatus.C_CONTRACT_STATUS_BEF_START,
                    CustomerType.C_CUST_TYPE_JURISTIC);
                List<CMS070_CustomerView> listView = new List<CMS070_CustomerView>();
                foreach (var i in list)
                {
                    i.CustCode = c.ConvertCustCode(i.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    CMS070_CustomerView view = CommonUtil.CloneObject<dtCustomerList, CMS070_CustomerView>(i);
                    listView.Add(view);
                }

                string xml = CommonUtil.ConvertToXml<CMS070_CustomerView>(listView, "Common\\CMS070_customer", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Search contract data by search criteria.
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public ActionResult CMS070_ContractSearch(doSearchInfoCondition searchCondition)
        {
            ObjectResultData res = new ObjectResultData();
            string customerStatus = ",";
            string customerType = ",";
            string serviceType = null;
            try
            {
                customerStatus += searchCondition.CustomerStatusNew == null ? "" : CustomerStatus.C_CUST_STATUS_NEW + ",";
                customerStatus += searchCondition.CustomerStatusExist == null ? "" : CustomerStatus.C_CUST_STATUS_EXIST + ",";
                if (customerStatus.Equals(","))
                    customerStatus = null;

                customerType += searchCondition.CustTypeJuristic == null ? "" : CustomerType.C_CUST_TYPE_JURISTIC + ",";
                customerType += searchCondition.CustTypeIndividual == null ? "" : CustomerType.C_CUST_TYPE_INDIVIDUAL + ",";
                customerType += searchCondition.CustTypeAssociation == null ? "" : CustomerType.C_CUST_TYPE_ASSOCIATION + ",";
                customerType += searchCondition.CustTypePublicOffice == null ? "" : CustomerType.C_CUST_TYPE_PUBLIC_OFFICE + ",";
                customerType += searchCondition.CustTypeOther == null ? "" : CustomerType.C_CUST_TYPE_OTHERS + ",";
                if (customerType.Equals(","))
                    customerType = null;

                if (searchCondition.SearchContractRental.HasValue && searchCondition.SearchContractRental.Value)
                {
                    serviceType = ServiceType.C_SERVICE_TYPE_RENTAL;
                }
                else if (searchCondition.SearchContractSale.HasValue && searchCondition.SearchContractSale.Value)
                {
                    serviceType = ServiceType.C_SERVICE_TYPE_SALE;
                }

                string RoleTypeContractTarget = null;
                string RoleTypePurchaser = null;
                string RoleTypeRealCustomer = null;

                if (CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET.Equals(searchCondition.CustomerRole))
                {
                    RoleTypeContractTarget = searchCondition.CustomerRole;
                }
                else if (CustRoleType.C_CUST_ROLE_TYPE_PURCHASER.Equals(searchCondition.CustomerRole))
                {
                    RoleTypePurchaser = searchCondition.CustomerRole;
                }
                else if (CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST.Equals(searchCondition.CustomerRole))
                {
                    RoleTypeRealCustomer = searchCondition.CustomerRole;
                }

                CommonUtil c = new CommonUtil();
                searchCondition.CustomerCode = c.ConvertCustCode(searchCondition.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                searchCondition.GroupCode = c.ConvertGroupCode(searchCondition.GroupCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                searchCondition.SiteCode = c.ConvertSiteCode(searchCondition.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                searchCondition.ContractCode = c.ConvertContractCode(searchCondition.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                searchCondition.ProjectCode = c.ConvertProjectCode(searchCondition.ProjectCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtContractList> list = hand.GetContractListForSearchInfo(
                    RoleTypeContractTarget,
                    RoleTypePurchaser,
                    RoleTypeRealCustomer,
                    serviceType,
                    searchCondition.CustomerCode,
                    searchCondition.GroupCode,
                    searchCondition.SiteCode,
                    searchCondition.ContractCode,
                    searchCondition.UserCode,
                    searchCondition.PlanCode,
                    searchCondition.ProjectCode,
                    searchCondition.CustomerName,
                    searchCondition.Branchename,
                    searchCondition.GroupName,
                    customerStatus,
                    customerType,
                    searchCondition.CompanyType,
                    searchCondition.CustomerIDNo,
                    searchCondition.CustomerNatioality,
                    searchCondition.CustomerBusinessType,
                    searchCondition.CustomerAddress,
                    searchCondition.CustomerSoi,
                    searchCondition.CustomerRoad,
                    searchCondition.CustomerTumbol,
                    searchCondition.CustomerJangwat,
                    searchCondition.CustomerAmper,
                    searchCondition.CustomerZipCode,
                    searchCondition.CustomerTelephone,
                    searchCondition.CustomerFax,
                    searchCondition.SiteName,
                    searchCondition.SiteAddress,
                    searchCondition.SiteSoi,
                    searchCondition.SiteRoad,
                    searchCondition.SiteTambol,
                    searchCondition.SiteJangwat,
                    searchCondition.SiteAmper,
                    searchCondition.SiteZipCode,
                    searchCondition.SiteTelephone,
                    searchCondition.OperationDateFrom,
                    searchCondition.OperationDateTo,
                    searchCondition.CustAcceptDateFrom,
                    searchCondition.CustAcceptDateTo,
                    searchCondition.CompleteDateFrom,
                    searchCondition.CompleteDateTo,
                    searchCondition.ContractOffice,
                    null, // Akat K. : Not use
                    searchCondition.OperationOffice,
                    searchCondition.SaleEmpNo,
                    searchCondition.SaleName,
                    searchCondition.ProductName,
                    searchCondition.ChangeType,
                    searchCondition.ProcessStatus,
                    searchCondition.StartType,
                    MiscType.C_RENTAL_CHANGE_TYPE,
                    MiscType.C_SALE_CHANGE_TYPE,
                    MiscType.C_SALE_PROC_MANAGE_STATUS,
                    CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET,
                    CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST,
                    CustRoleType.C_CUST_ROLE_TYPE_PURCHASER,
                    FlagType.C_FLAG_ON,
                    ServiceType.C_SERVICE_TYPE_RENTAL,
                    ServiceType.C_SERVICE_TYPE_SALE,
                    ContractStatus.C_CONTRACT_STATUS_BEF_START,
                    ContractStatus.C_CONTRACT_STATUS_CANCEL,
                    ContractStatus.C_CONTRACT_STATUS_END,
                    SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE,
                    CustomerType.C_CUST_TYPE_JURISTIC,
                    searchCondition.StopDateFrom,
                    searchCondition.StopDateTo,
                    searchCondition.CancelDateFrom,
                    searchCondition.CancelDateTo);

                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                CommonUtil.MappingObjectLanguage<dtContractList>(list);
                List<CMS070_ContractView> listView = new List<CMS070_ContractView>();

                string unoperated = CommonUtil.GetLabelFromResource("Common", "CMS070", "lblChangeTypeSubfixUnoperate");
                string operated = CommonUtil.GetLabelFromResource("Common", "CMS070", "lblChangeTypeSubfixOperate");
                string cancel = CommonUtil.GetLabelFromResource("Common", "CMS070", "lblChangeTypeSubfixCancel");

                foreach (dtContractList i in list)
                {
                    string changeTypeSubifx = "";

                    if (ServiceType.C_SERVICE_TYPE_SALE.Equals(i.ServiceTypeCode)
                        && (SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE.Equals(i.ChangeType)
                            || SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE.Equals(i.ChangeType)))
                    {
                        if (ContractStatus.C_CONTRACT_STATUS_BEF_START.Equals(i.ContractStatus))
                        {
                            changeTypeSubifx = unoperated;
                        }
                        else if (ContractStatus.C_CONTRACT_STATUS_AFTER_START.Equals(i.ContractStatus)
                          || ContractStatus.C_CONTRACT_STATUS_STOPPING.Equals(i.ContractStatus)
                          || ContractStatus.C_CONTRACT_STATUS_END.Equals(i.ContractStatus))
                        {
                            changeTypeSubifx = operated;
                        }
                        else if (ContractStatus.C_CONTRACT_STATUS_CANCEL.Equals(i.ContractStatus)
                          || ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL.Equals(i.ContractStatus))
                        {
                            changeTypeSubifx = cancel;
                        }
                    }

                    //i.ContractNameDisplay = i.ContractNameDisplay + "<br/>" + i.SiteNameDisplay;
                    i.ContractNameDisplay = "(1) " + (CommonUtil.IsNullOrEmpty(i.CustNameEN) ? "-" : i.CustNameEN) + "<br/>";
                    i.ContractNameDisplay += "(2) " + (CommonUtil.IsNullOrEmpty(i.CustNameLC) ? "-" : i.CustNameLC) + "<br/>";
                    i.ContractNameDisplay += "(3) " + (CommonUtil.IsNullOrEmpty(i.SiteNameEN) ? "-" : i.SiteNameEN) + "<br/>";
                    i.ContractNameDisplay += "(4) " + (CommonUtil.IsNullOrEmpty(i.SiteNameLC) ? "-" : i.SiteNameLC);

                    //if (i.OperationDate.HasValue) {
                    //    i.ChangeTypeNameDisplay = "(1) " + i.ChangeType + ":" + i.ChangeTypeName + changeTypeSubifx
                    //                                + "<br/>(2) " + i.OperationDate.Value.ToString("dd-MMM-yyyy");
                    //} else {
                    //    i.ChangeTypeNameDisplay = "(1) " + i.ChangeType + ":" + i.ChangeTypeName + changeTypeSubifx
                    //                                + "<br/>(2) -";
                    //}

                    i.ChangeTypeNameDisplay = string.Format("(1) {0}<br>(2) {1}<br>(3) {2}<br>(4) {3}"
                        , i.ChangeType + ":" + i.ChangeTypeName + changeTypeSubifx
                        , i.OperationDate.HasValue ? i.OperationDate.Value.ToString("dd-MMM-yyyy") : "-"
                        , i.StopProcessDate.HasValue ? i.StopProcessDate.Value.ToString("dd-MMM-yyyy") : "-"
                        , i.StopConditionProcessDate.HasValue ? i.StopConditionProcessDate.Value.ToString("dd-MMM-yyyy") : "-"
                    );

                    if (i.ProductCode != null)
                    {
                        i.ProductNameDisplay = "(1) " + i.ProductCode;
                        if (i.ProductName != null)
                        {
                            i.ProductNameDisplay += ":" + i.ProductName + "<br/>(2) ";
                        }
                        else
                        {
                            i.ProductNameDisplay += ": -<br/>(2) ";
                        }
                    }
                    else
                    {
                        i.ProductNameDisplay = "(1) -<br/>(2) ";
                    }

                    if (i.Price.HasValue)
                    {
                        i.ProductNameDisplay = i.ProductNameDisplay + i.TextPrice;
                    }
                    else
                    {
                        i.ProductNameDisplay += "-";
                    }

                    //i.OfficeDisplay = "(1) " + i.ContractOfficeName + "<br/>(2) " + i.OperationOfficeName;
                    i.OfficeDisplay = "(1) " + (CommonUtil.IsNullOrEmpty(i.ContractOfficeName) ? "-" : i.ContractOfficeName) + "<br/>";
                    i.OfficeDisplay += "(2) " + (CommonUtil.IsNullOrEmpty(i.OperationOfficeName) ? "-" : i.OperationOfficeName);

                    i.SaleProcessManageStatusDisplay = i.SaleProcessManageStatusName;

                    i.SiteCode = c.ConvertSiteCode(i.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    i.ContractCode = c.ConvertContractCode(i.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    CMS070_ContractView view = CommonUtil.CloneObject<dtContractList, CMS070_ContractView>(i);
                    listView.Add(view);
                }

                string xml = CommonUtil.ConvertToXml<CMS070_ContractView>(listView, "Common\\CMS070_contract", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get detail of ProductName combobox when user change service type.
        /// </summary>
        /// <param name="ServiceTypeCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CMS070_GetProductTypeByProductTypeCode(string ServiceTypeCode)
        {

            string strDisplayName = "ValueCodeDisplay";
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<View_tbm_Product> list = new List<View_tbm_Product>();
                try
                {
                    IProductMasterHandler handler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;

                    list = handler.GetTbm_ProductByLanguage(null, null);
                }
                catch
                {
                    list = new List<View_tbm_Product>();
                }

                if (!CommonUtil.IsNullOrEmpty(ServiceTypeCode))
                {
                    if (ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        list = (from t in list
                                where t.ProductTypeCode == ProductType.C_PROD_TYPE_SALE
                                select t).ToList<View_tbm_Product>();
                    }
                    else if (ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        list = (from t in list
                                where t.ProductTypeCode != ProductType.C_PROD_TYPE_SALE
                                select t).ToList<View_tbm_Product>();
                    }

                }


                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<View_tbm_Product>(list, "ProductCodeName", "ProductCode", true, CommonUtil.eFirstElementType.All);

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
    }


}
