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
        private const string CMS080_Screen = "CMS080";

        /// <summary>
        /// Check user permission for screen CMS080.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult CMS080_Authority(CMS080_ScreenParameter obj)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_INFO, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                obj.strCustomerCode_short = obj.strCustomerCode;

                CommonUtil c = new CommonUtil();
                obj.strCustomerCode = c.ConvertCustCode(obj.strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                List<doCustomer> custList = handler.GetCustomerAll(obj.strCustomerCode);

                if (custList.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                obj.hasPermission_CMS100 = CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, FunctionID.C_FUNC_ID_OPERATE);
                obj.hasPermission_CMS280 = CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_SITE_INFO, FunctionID.C_FUNC_ID_OPERATE);

                return InitialScreenEnvironment<CMS080_ScreenParameter>(CMS080_Screen, obj, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen and get customer information.
        /// </summary>
        /// <returns></returns>
        [Initialize(CMS080_Screen)]
        public ActionResult CMS080()
        {
            CMS080_ScreenParameter param = new CMS080_ScreenParameter();
            ViewBag.HasPermissionCMS100 = "";
            ViewBag.HasPermissionCMS280 = "";
           
            try
            {
                param = GetScreenObject<CMS080_ScreenParameter>();
                ViewBag.HasPermissionCMS100 = param.hasPermission_CMS100;
                ViewBag.HasPermissionCMS280 = param.hasPermission_CMS280;
                ViewBag.CustCode_short = param.strCustomerCode_short;

            }
            catch
            {
            }

            CommonUtil c = new CommonUtil();

            try
            {
                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                List<doCustomer> custList = handler.GetCustomerAll(param.strCustomerCode);

                if (custList.Count != 0)
                {
                    ViewBag.CustCode = c.ConvertCustCode(custList[0].CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.CustNameEN = custList[0].CustFullNameEN;
                    ViewBag.AddressFullEN = custList[0].AddressFullEN;
                    ViewBag.CustNameLC = custList[0].CustFullNameLC;
                    ViewBag.AddressFullLC = custList[0].AddressFullLC;
                    ViewBag.RepPersonName = custList[0].RepPersonName;
                    ViewBag.ContactPersonName = custList[0].ContactPersonName;
                    ViewBag.URL = custList[0].URL;
                    ViewBag.IDNo = custList[0].IDNo;
                    ViewBag.PhoneNo = custList[0].PhoneNo;
                    ViewBag.SECOMContactPerson = custList[0].SECOMContactPerson;
                    ViewBag.FaxNo = custList[0].FaxNo;
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        ViewBag.CustType = CommonUtil.TextCodeName(custList[0].CustTypeCode, custList[0].CustTypeNameEN);
                        ViewBag.FinancialMarketTypeName = CommonUtil.TextCodeName(custList[0].FinancialMarketTypeCode, custList[0].FinancialMaketTypeNameEN);
                        ViewBag.Nationality = custList[0].NationalityEN;
                        ViewBag.BusinessTypeName = CommonUtil.TextCodeName(custList[0].BusinessTypeCode, custList[0].BusinessTypeNameEN);
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        ViewBag.CustType = CommonUtil.TextCodeName(custList[0].CustTypeCode, custList[0].CustTypeNameJP);
                        ViewBag.FinancialMarketTypeName = CommonUtil.TextCodeName(custList[0].FinancialMarketTypeCode, custList[0].FinancialMaketTypeNameJP);
                        ViewBag.Nationality = custList[0].NationalityJP;
                        ViewBag.BusinessTypeName = CommonUtil.TextCodeName(custList[0].BusinessTypeCode, custList[0].BusinessTypeNameJP);
                    }
                    else
                    {
                        ViewBag.CustType = CommonUtil.TextCodeName(custList[0].CustTypeCode, custList[0].CustTypeNameLC);
                        ViewBag.FinancialMarketTypeName = CommonUtil.TextCodeName(custList[0].FinancialMarketTypeCode, custList[0].FinancialMaketTypeNameLC);
                        ViewBag.Nationality = custList[0].NationalityLC;
                        ViewBag.BusinessTypeName = CommonUtil.TextCodeName(custList[0].BusinessTypeCode, custList[0].BusinessTypeNameLC);
                    }
                    if (custList[0].DeleteFlag.Value)
                    {
                        ViewBag.Deleted = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_INFO, "lblDeleted");
                    }

                    string rolelabel = CommonUtil.GetLabelFromResource("Common", "CMS080", "txtRole");
                    if (CommonUtil.IsNullOrEmpty(param.strCustomerRole))
                    {
                        ViewBag.CustomerRoleLabel = "(" + rolelabel + ": " + CommonUtil.GetLabelFromResource("Common", "CMS080", "txtRoleAll") + ")";
                    }
                    else if (param.strCustomerRole == "1")
                    {
                            ViewBag.CustomerRoleLabel = "(" + rolelabel + ": Contract)";
                    }
                    else{
                        ICommonHandler commonHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<doMiscTypeCode> misc = new List<doMiscTypeCode>();
                        misc.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_CUST_ROLE_TYPE,
                            ValueCode = param.strCustomerRole
                        });

                        List<doMiscTypeCode> roleType = commonHand.GetMiscTypeCodeList(misc);
                        if (roleType.Count != 0)
                        {
                            ViewBag.CustomerRoleLabel = "(" + rolelabel + ": " + roleType[0].ValueDisplay + ")";
                        }
                    }
                }

                ViewBag.CustomerCode = param.strCustomerCode;
                ViewBag.CustomerRole = CommonUtil.IsNullOrEmpty(param.strCustomerRole) ? "" : param.strCustomerRole;

                return View();
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return null;
            }

        }

        /// <summary>
        /// Get config for Site List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS080_InitSiteGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS080_site_list"));
        }

        /// <summary>
        /// Get config for Group List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS080_InitGroupGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS080_group_list"));
        }

        /// <summary>
        /// Get group information of customer.
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public ActionResult CMS080_CustomerGroupSearch(string custCode)
        {
            //CMS080_ScreenParameter param = GetScreenObject<CMS080_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            try
            {
                ICustomerMasterHandler hand = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                List<dtCustomerGroupForView> list = hand.GetCustomerGroup(null, custCode);
                string xml = CommonUtil.ConvertToXml<dtCustomerGroupForView>(list, "Common\\CMS080_group_list", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Get site information of customer.
        /// </summary>
        /// <param name="custCode"></param>
        /// <param name="strCustomerRole"></param>
        /// <returns></returns>
        public ActionResult CMS080_SiteSearch(string custCode, string strCustomerRole)
        {
            //CMS080_ScreenParameter param = GetScreenObject<CMS080_ScreenParameter>();
            CommonUtil c = new CommonUtil();
            strCustomerRole = CommonUtil.IsNullOrEmpty(strCustomerRole) ? null : strCustomerRole;

            ObjectResultData res = new ObjectResultData();
            try
            {
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtSiteList> list = hand.GetSiteListForCustInfo(
                    custCode,
                    strCustomerRole,
                    CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET,
                    CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST,
                    CustRoleType.C_CUST_ROLE_TYPE_PURCHASER,
                    FlagType.C_FLAG_ON,
                    ServiceType.C_SERVICE_TYPE_RENTAL,
                    RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT,
                    RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL,
                    ServiceType.C_SERVICE_TYPE_SALE,
                    ContractStatus.C_CONTRACT_STATUS_CANCEL,
                    ContractStatus.C_CONTRACT_STATUS_END,
                    ContractStatus.C_CONTRACT_STATUS_BEF_START,
                    ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);

                foreach (var i in list)
                {
                    i.SiteCode = c.ConvertSiteCode(i.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }

                string xml = CommonUtil.ConvertToXml<dtSiteList>(list, "Common\\CMS080_site_list", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
    }
}
